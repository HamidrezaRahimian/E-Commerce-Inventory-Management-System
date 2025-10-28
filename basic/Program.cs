using System;
using System.Linq;
using ECommerceInventorySystem.Models;
using ECommerceInventorySystem.Warehouse;
using ECommerceInventorySystem.Utils;

namespace ECommerceInventorySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Lager initialisieren (MainWarehouse als Beispiel)
            var warehouse = new Warehouse<Product>
            {
                Location = WarehouseLocation.MainWarehouse
            };

            // Mit Musterprodukten und Verkaufshistorie füllen
            WarehouseSeeder.SeedProducts(warehouse, 50);      // 50 zufällige Produkte erzeugen
            WarehouseSeeder.SeedSalesHistory(warehouse, 200); // 200 zufällige Verkäufe erzeugen

            // Aufgabe a): Produkte einer Kategorie finden, deren Bestand kritisch ist und die in den letzten 30 Tagen verkauft wurden
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

            Console.WriteLine("Ausgabe aufgabe a):");

            if (kritischsteProdukte.Count == 0)
            {
                Console.WriteLine("Es wurde nichts gefunden.");
            }
            else
            {
                Console.WriteLine("Kritischste Produkte Wurden Gefunden:");
                foreach (var item in kritischsteProdukte)
                {
                    Console.WriteLine($"Name: {item.Produktname}, SKU: {item.SKU}, Bestand: {item.AktuellerBestand}, Mindestbestand: {item.Mindestbestand}, Diff: {item.Differenz}, Letzter Verkauf: {(item.LetztesVerkaufsdatum.HasValue ? item.LetztesVerkaufsdatum.Value.ToString("yyyy-MM-dd") : "nie")}");
                }
            }
            // Aufgabe b): Lagerumschlagsrate pro Produktkategorie für das letzte Halbjahr berechnen

            var halbjahrBeginn = DateTime.Now.AddMonths(-6);

            // Alle relevanten Verkäufe im letzten Halbjahr
            var halbjahresVerkaeufe = warehouse.SalesHistory
                .Where(s => s.SoldDate >= halbjahrBeginn)
                .ToList();

            // Gruppiere Verkäufe nach Kategorie und berechne Summen und Durchschnitte
            var umschlagsraten = warehouse.Products
                .GroupBy(p => p.Category)
                .Select(g =>
                {
                    var category = g.Key;
                    var skusInCategory = g.Select(p => p.SKU).ToHashSet();

                    // Alle Verkäufe für die SKUs dieser Kategorie
                    var sales = halbjahresVerkaeufe.Where(s => skusInCategory.Contains(s.SKU)).ToList();
                    var totalSold = sales.Sum(s => s.Quantity);

                    // Durchschnittlicher Bestand: Mittelwert aller aktuellen Bestände aus warehouse.Products in der Kategorie
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

            // Ausgabe für Aufgabe b)
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


            // Aufgabe c): 15 profitabelste Produkte nach Gesamtmarge im aktuellen Quartal

            // Hilfsfunktion: Quartalsbeginn berechnen
            DateTime GetQuarterStart(DateTime now)
            {
                int quarter = (now.Month - 1) / 3 + 1;
                int startMonth = 3 * (quarter - 1) + 1;
                return new DateTime(now.Year, startMonth, 1);
            }

            var quartalBeginn = GetQuarterStart(DateTime.Now);

            // Alle Verkäufe dieses Quartals
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

            // Ausgabe für Aufgabe c)
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


        }
    }
}
