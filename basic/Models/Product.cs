using System;

namespace ECommerceInventorySystem.Models
{
    public class Product : IStorable
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProductCategory Category { get; set; }
        public ProductCondition Condition { get; set; }
        public decimal Price { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Supplier { get; set; }
        public int Quantity { get; set; }
        public int MinimumStock { get; set; }


        public override string ToString() =>
            $"{SKU} - {Name} [{Category}] ({Quantity}x, {Price:C}, Zustand: {Condition})";
    }
}
