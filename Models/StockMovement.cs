using System;

namespace ECommerceInventorySystem.Models
{
    public class StockMovement
    {
        public string SKU { get; set; }
        public int Change { get; set; }
        public DateTime Timestamp { get; set; }
        public string Reason { get; set; }
    }
}
