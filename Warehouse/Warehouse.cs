using System.Collections.Generic;
using ECommerceInventorySystem.Models;

namespace ECommerceInventorySystem.Warehouse
{
    public class Warehouse<T> where T : IStorable
    {
        public WarehouseLocation Location { get; set; }
        public T[] ShelfLevels { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public HashSet<string> UniqueSKUs { get; set; } = new HashSet<string>();
        public Dictionary<string, InventoryLevel> StockLevels { get; set; } = new Dictionary<string, InventoryLevel>();
        public Queue<RestockOrder> RestockOrders { get; set; } = new Queue<RestockOrder>();
        public Stack<StockMovement> StockMovements { get; set; } = new Stack<StockMovement>();
        public List<SalesTransaction> SalesHistory { get; set; } = new List<SalesTransaction>();    
    }
}
