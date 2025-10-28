namespace ECommerceInventorySystem.Models
{
    public class SalesTransaction
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public DateTime SoldDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public WarehouseLocation Location { get; set; }
    }
}
