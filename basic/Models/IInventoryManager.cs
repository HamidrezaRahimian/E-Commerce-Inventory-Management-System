using System.Collections.Generic;

namespace ECommerceInventorySystem.Models
{
    public interface IStorable
    {
        string SKU { get; }
        int Quantity { get; set; }
    }

    public interface IInventoryManager
    {
        void AddStock(string sku, int quantity);
        void RemoveStock(string sku, int quantity);
        void TransferStock(string sku, int quantity, WarehouseLocation location);
        int CheckStockLevel(string sku);
        int CalculateReorderPoint(string sku); // Standard
        int CalculateReorderPoint(string sku, double seasonalFactor); // Saisonale Anpassung
        int CalculateReorderPoint(string sku, List<int> historicalSales); // Historische Daten
    }
}
