namespace ECommerceInventorySystem.Models
{
    public interface IStorable
    {
        string SKU { get; }
        int Quantity { get; set; }
    }
}
