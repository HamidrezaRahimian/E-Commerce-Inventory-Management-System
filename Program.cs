using System;
using System.Linq;
using System.Collections.Generic;
using ECommerceInventorySystem.Models;
using ECommerceInventorySystem.Warehouse;
using ECommerceInventorySystem.Utils;

namespace ECommerceInventorySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init single warehouse instance
            var warehouse = new Warehouse<Product>
            {
                Location = WarehouseLocation.MainWarehouse
            };

            // Seed demo data
            WarehouseSeeder.SeedProducts(warehouse, 50);
            WarehouseSeeder.SeedSalesHistory(warehouse, 200);

            // ---------- Aufgabe a ----------
            var selectedCategory = ProductCategory.Electronics;
            var dateThreshold = DateTime.Now.AddDays(-30);

            var kritischsteProdukte = warehouse.Products
                .Where(p =>
                    p.Category == selectedCategory &&
                    p.Quantity < p.MinimumStock &&
                    warehouse.SalesHistory.Any(s =>
                        s.SKU == p.SKU &&
                        s.SoldDate >= dateThreshold
                    )
                )
                .Select(p => new
                {
                    Produktname = p.Name,
                    SKU = p.SKU,
                    AktuellerBestand = p.Quantity,
                    Mindestbestand = p.MinimumStock,
                    Differenz = p.Quantity - p.MinimumStock,
                    LetztesVerkaufsdatum = warehouse.SalesHistory
                        .Where(s => s.SKU == p.SKU)
                        .OrderByDescending(s => s.SoldDate)
                        .Select(s => (DateTime?)s.SoldDate)
                        .FirstOrDefault()
                })
                .OrderBy(x => x.Differenz)
                .ToList();

            Console.WriteLine("Ausgabe Aufgabe a):");

            if (kritischsteProdukte.Count == 0)
            {
                Console.WriteLine("Es wurde nichts gefunden.");
            }
            else
            {
                Console.WriteLine("Kritischste Produkte wurden gefunden:");
                foreach (var item in kritischsteProdukte)
                {
                    Console.WriteLine($"Name: {item.Produktname}, SKU: {item.SKU}, Bestand: {item.AktuellerBestand}, Mindestbestand: {item.Mindestbestand}, Diff: {item.Differenz}, Letzter Verkauf: {(item.LetztesVerkaufsdatum.HasValue ? item.LetztesVerkaufsdatum.Value.ToString("yyyy-MM-dd") : "nie")}");
                }
            }

            // ---------- Aufgabe b ----------
            var halbjahrBeginn = DateTime.Now.AddMonths(-6);
            var halbjahresVerkaeufe = warehouse.SalesHistory
                .Where(s => s.SoldDate >= halbjahrBeginn)
                .ToList();

            var umschlagsraten = warehouse.Products
                .GroupBy(p => p.Category)
                .Select(g =>
                {
                    var category = g.Key;
                    var skusInCategory = g.Select(p => p.SKU).ToHashSet();

                    var sales = halbjahresVerkaeufe.Where(s => skusInCategory.Contains(s.SKU)).ToList();
                    var totalSold = sales.Sum(s => s.Quantity);

                    var avgStock = g.Average(p => (double)p.Quantity);
                    double turnoverRate = avgStock > 0 ? (double)totalSold / avgStock : 0;

                    return new
                    {
                        Kategorie = category,
                        Gesamtverkaeufe = totalSold,
                        DurchschnittlicherBestand = avgStock,
                        Umschlagsrate = turnoverRate
                    };
                })
                .OrderByDescending(x => x.Umschlagsrate)
                .ToList();

            Console.WriteLine("\nAusgabe Aufgabe b):");
            if (umschlagsraten.Count == 0)
            {
                Console.WriteLine("Keine Umschlagsraten ermittelbar.");
            }
            else
            {
                Console.WriteLine("Umschlagsraten pro Kategorie (letzte 6 Monate):");
                foreach (var k in umschlagsraten)
                {
                    Console.WriteLine($"Kategorie: {k.Kategorie}, Gesamtverkäufe: {k.Gesamtverkaeufe}, Durchschnittlicher Bestand: {k.DurchschnittlicherBestand:N2}, Umschlagsrate: {k.Umschlagsrate:N2}");
                }
            }

            // ---------- Aufgabe c ----------
            DateTime GetQuarterStart(DateTime now)
            {
                int quarter = (now.Month - 1) / 3 + 1;
                int startMonth = 3 * (quarter - 1) + 1;
                return new DateTime(now.Year, startMonth, 1);
            }

            var quartalBeginn = GetQuarterStart(DateTime.Now);

            var quartalSales = warehouse.SalesHistory
                .Where(s => s.SoldDate >= quartalBeginn)
                .ToList();

            var profitableProducts = quartalSales
                .GroupBy(s => s.SKU)
                .Select(g =>
                {
                    var totalMargin = g.Sum(s => (s.SalePrice - s.PurchasePrice) * s.Quantity);
                    var totalSold = g.Sum(s => s.Quantity);
                    var product = warehouse.Products.FirstOrDefault(p => p.SKU == g.Key);

                    return new
                    {
                        SKU = g.Key,
                        Name = product?.Name ?? "Unbekannt",
                        Kategorie = product?.Category.ToString() ?? "Unbekannt",
                        AktuellerBestand = product?.Quantity ?? 0,
                        Gesamtmarge = totalMargin,
                        Gesamtverkaeufe = totalSold
                    };
                })
                .OrderByDescending(x => x.Gesamtmarge)
                .Take(15)
                .ToList();

            Console.WriteLine("\nAusgabe Aufgabe c):");
            if (profitableProducts.Count == 0)
            {
                Console.WriteLine("Keine profitablen Produkte im aktuellen Quartal gefunden.");
            }
            else
            {
                Console.WriteLine("Top 15 profitabelste Produkte im aktuellen Quartal:");
                foreach (var p in profitableProducts)
                {
                    Console.WriteLine($"SKU: {p.SKU}, Name: {p.Name}, Kategorie: {p.Kategorie}, Bestand: {p.AktuellerBestand}, Gesamtmarge: {p.Gesamtmarge:N2}, Verkäufe: {p.Gesamtverkaeufe}");
                }
            }

            // =====================================================================
            // ========================= Aufgabe d =================================
            // =====================================================================
            // Find all products with ExpiryDate in next 30 days, group by WarehouseLocation,
            // exclude Discontinued, show count and potential loss value,
            // and list critical items sorted by potential loss desc.
            Console.WriteLine("\nAusgabe Aufgabe d):");

            var now = DateTime.Now;
            var in30Days = now.AddDays(30);

            // helper to infer a stock status without changing the data model
            StockStatus InferStatus(Product p)
            {
                if (p.Quantity <= 0) return StockStatus.OutOfStock;
                if (p.Quantity < p.MinimumStock) return StockStatus.LowStock;
                return StockStatus.InStock;
            }

            var expiringByLocation = warehouse.Products
                .Where(p =>
                    p.ExpiryDate.HasValue &&
                    p.ExpiryDate.Value <= in30Days &&
                    InferStatus(p) != StockStatus.Discontinued // exclude Discontinued
                )
                .Select(p => new
                {
                    Location = warehouse.Location, // single location in current model
                    Product = p,
                    PotentialLoss = p.Price * p.Quantity // simple potential loss estimate
                })
                .GroupBy(x => x.Location)
                .Select(g => new
                {
                    Standort = g.Key,
                    AnzahlAblaufend = g.Count(),
                    Gesamtwert = g.Sum(x => x.PotentialLoss),
                    KritischsteArtikel = g
                        .OrderByDescending(x => x.PotentialLoss)
                        .Take(5)
                        .Select(x => new
                        {
                            x.Product.SKU,
                            x.Product.Name,
                            Bestand = x.Product.Quantity,
                            Preis = x.Product.Price,
                            Ablaufdatum = x.Product.ExpiryDate?.ToString("yyyy-MM-dd"),
                            Verlust = x.PotentialLoss
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.Gesamtwert)
                .ToList();

            if (expiringByLocation.Count == 0)
            {
                Console.WriteLine("Keine ablaufenden Produkte in den nächsten 30 Tagen gefunden.");
            }
            else
            {
                foreach (var grp in expiringByLocation)
                {
                    Console.WriteLine($"Standort: {grp.Standort}, Anzahl ablaufend: {grp.AnzahlAblaufend}, Potenzieller Verlust Gesamt: {grp.Gesamtwert:C}");
                    Console.WriteLine("Kritischste Artikel:");
                    foreach (var it in grp.KritischsteArtikel)
                    {
                        Console.WriteLine($"  SKU: {it.SKU}, Name: {it.Name}, Bestand: {it.Bestand}, Preis: {it.Preis:C}, Ablaufdatum: {it.Ablaufdatum}, Verlust: {it.Verlust:C}");
                    }
                }
            }

            // =====================================================================
            // ========================= Aufgabe e =================================
            // =====================================================================
            // Reorder report per category
            // For each category:
            //   - number of items under min stock
            //   - average shortage (min - current)
            //   - estimated restock costs
            //   - urgency based on sales velocity (avg daily sales per SKU)
            Console.WriteLine("\nAusgabe Aufgabe e):");

            int lookbackDays = 30;
            var salesSince = now.AddDays(-lookbackDays);

            // build sales aggregates for the lookback window
            var salesAgg = warehouse.SalesHistory
                .Where(s => s.SoldDate >= salesSince)
                .GroupBy(s => s.SKU)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        TotalSold = g.Sum(x => x.Quantity),
                        AvgPurchase = g.Any() ? g.Average(x => x.PurchasePrice) : 0m
                    }
                );

            // items needing reorder
            var lowOrOut = warehouse.Products
                .Where(p =>
                    p.Quantity < p.MinimumStock // LowStock or OutOfStock
                )
                .ToList();

            var reorderReport = lowOrOut
                .GroupBy(p => p.Category)
                .Select(g =>
                {
                    var items = g.ToList();
                    int itemCount = items.Count;
                    double avgShortage = itemCount == 0 ? 0.0 :
                        items.Average(p => Math.Max(p.MinimumStock - p.Quantity, 0));

                    // estimated cost = sum(shortage * unitCost), unitCost from AvgPurchase if available else 60% of Price
                    decimal estimatedCost = items.Sum(p =>
                    {
                        int shortage = Math.Max(p.MinimumStock - p.Quantity, 0);
                        decimal unitCost =
                            salesAgg.TryGetValue(p.SKU, out var agg) && agg.AvgPurchase > 0m
                                ? agg.AvgPurchase
                                : p.Price * 0.6m;
                        return unitCost * shortage;
                    });

                    // daily sales rate per SKU in this category over lookback window
                    // if no sales, treat as 0
                    double avgDailyRate = itemCount == 0 ? 0.0 :
                        items.Average(p =>
                            salesAgg.TryGetValue(p.SKU, out var agg)
                                ? (double)agg.TotalSold / lookbackDays
                                : 0.0
                        );

                    // urgency: higher with bigger shortage and higher daily sales velocity
                    double urgencyScore = avgShortage * (avgDailyRate + 0.01); // +epsilon to rank pure shortage slightly

                    return new
                    {
                        Kategorie = g.Key,
                        AnzahlArtikel = itemCount,
                        DurchschnittlicherFehlbestand = avgShortage,
                        GeschaetzteNachbestellkosten = estimatedCost,
                        VerkaufsrateProTag = avgDailyRate,
                        Dringlichkeit = urgencyScore
                    };
                })
                .OrderByDescending(x => x.Dringlichkeit)
                .ToList();

            if (reorderReport.Count == 0)
            {
                Console.WriteLine("Keine Artikel unter Mindestbestand gefunden.");
            }
            else
            {
                Console.WriteLine($"Nachbestellbericht pro Kategorie (Fenster: {lookbackDays} Tage):");
                foreach (var row in reorderReport)
                {
                    Console.WriteLine(
                        $"Kategorie: {row.Kategorie}, " +
                        $"Anzahl: {row.AnzahlArtikel}, " +
                        $"Ø Fehlbestand: {row.DurchschnittlicherFehlbestand:N2}, " +
                        $"Kosten: {row.GeschaetzteNachbestellkosten:C}, " +
                        $"Verkaufsrate/Tag: {row.VerkaufsrateProTag:N2}, " +
                        $"Dringlichkeit: {row.Dringlichkeit:N4}"
                    );
                }
            }
        }
    }
}
