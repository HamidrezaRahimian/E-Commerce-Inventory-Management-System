using System;

namespace ECommerceInventorySystem.Models
{
    public class RestockOrder
    {
        public string SKU { get; set; }
        public int Amount { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ExpectedDelivery { get; set; }
    }
}
