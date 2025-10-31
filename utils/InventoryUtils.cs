using System;
using System.Linq;
using ECommerceInventorySystem.Models;

namespace ECommerceInventorySystem.Utils
{
    public static class InventoryUtils
    {
        public static bool IsValidSKU(string sku) =>
            !string.IsNullOrEmpty(sku) && sku.All(char.IsLetterOrDigit);

        public static string ComposeDescription(Product product) =>
            $"{product.Name}: {product.Description} ({product.Category}, {product.Condition})";

        public static string GenerateBarcode(string sku) =>
            $"*{sku}-BARCODE*";

        public static string FormatWarehouseCode(WarehouseLocation location, int shelf) =>
            $"{location}-{shelf:D3}";

        public static double CalculateTurnoverRate(int soldUnits, int avgStock) =>
            avgStock == 0 ? 0 : (double)soldUnits / avgStock;

        public static int CalculateStockDuration(DateTime receivedDate, DateTime now) =>
            (now - receivedDate).Days;

        public static bool IsExpired(Product product, DateTime now) =>
            product.ExpiryDate.HasValue && product.ExpiryDate.Value < now;

        public static DateTime GetReorderPoint(DateTime lastRestock, int daysInterval) =>
            lastRestock.AddDays(daysInterval);
    }
}
