using System;
using System.Collections.Generic;
using System.Linq;
using ECommerceInventorySystem.Models;

namespace ECommerceInventorySystem.Services
{
    public class InventoryManager : IInventoryManager
    {
        private Dictionary<string, InventoryLevel> stockLevels;

        public InventoryManager(Dictionary<string, InventoryLevel> levels)
        {
            stockLevels = levels;
        }

        public void AddStock(string sku, int quantity)
        {
            if (!stockLevels.ContainsKey(sku))
                stockLevels[sku] = new InventoryLevel { Quantity = 0, LastUpdated = DateTime.Now };
            stockLevels[sku].Quantity += quantity;
            stockLevels[sku].LastUpdated = DateTime.Now;
        }

        public void RemoveStock(string sku, int quantity)
        {
            if (stockLevels.ContainsKey(sku))
            {
                stockLevels[sku].Quantity = Math.Max(stockLevels[sku].Quantity - quantity, 0);
                stockLevels[sku].LastUpdated = DateTime.Now;
            }
        }

        public void TransferStock(string sku, int quantity, WarehouseLocation location)
        {
            // Hier kann die Lagerortlogik erweitert werden
            RemoveStock(sku, quantity);
        }

        public int CheckStockLevel(string sku) =>
            stockLevels.ContainsKey(sku) ? stockLevels[sku].Quantity : 0;

        public int CalculateReorderPoint(string sku)
        {
            // Beispielhafte Standardformel
            int leadTime = 7;
            int dailyUsage = 10;
            return leadTime * dailyUsage;
        }
        public int CalculateReorderPoint(string sku, double seasonalFactor)
        {
            int basePoint = CalculateReorderPoint(sku);
            return (int)(basePoint * seasonalFactor);
        }
        public int CalculateReorderPoint(string sku, List<int> historicalSales)
        {
            int avg = historicalSales.Count > 0 ? (int)historicalSales.Average() : 10;
            int leadTime = 7;
            return leadTime * avg;
        }
    }
}
