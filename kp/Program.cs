using System.Globalization;
using CsvHelper;

class Program
{
    static void Main()
    {
        // Produkte laden
        List<Product> products;
        using (var reader = new StreamReader("products.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            products = csv.GetRecords<Product>().ToList();
        }

        // Verkäufe laden
        List<Sale> sales;
        using (var reader = new StreamReader("sales.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            sales = csv.GetRecords<Sale>().ToList();
        }

        // Suchparameter setzen
        var daysBack = 30;
        var selectedCategory = "Electronics";
        var cutoffDate = DateTime.Now.AddDays(-daysBack);

        // Vorbereiten: Zuordnung ProduktId zu letztem Verkaufsdatum
        var recentSales = sales
            .Where(s => s.Date >= cutoffDate)
            .GroupBy(s => s.ProductId)
            .Select(g => new 
            {
                ProductId = g.Key,
                LastSaleDate = g.Max(s => s.Date)
            })
            .ToDictionary(x => x.ProductId, x => x.LastSaleDate);

        // Hauptabfrage
        var result = products
            .Where(p => p.Category == selectedCategory && p.CurrentStock < p.MinimumStock && recentSales.ContainsKey(p.Id))
            .Select(p => new
            {
                Produktname = p.Name,
                SKU = p.SKU,
                AktuellerBestand = p.CurrentStock,
                Mindestbestand = p.MinimumStock,
                Differenz = p.CurrentStock - p.MinimumStock,
                LetztesVerkaufsdatum = recentSales[p.Id]
            })
            .OrderBy(x => x.Differenz)
            .ToList();

        // Ausgabe
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Produktname} | {item.SKU} | Bestand: {item.AktuellerBestand} | Minimum: {item.Mindestbestand} | Delta: {item.Differenz} | Verkauf: {item.LetztesVerkaufsdatum}");
        }
    }
}
