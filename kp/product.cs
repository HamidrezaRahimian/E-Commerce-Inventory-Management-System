public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public string Category { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string WarehouseLocation { get; set; }
    public string StockStatus { get; set; }
    public DateTime ExpirationDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalesPrice { get; set; }
}