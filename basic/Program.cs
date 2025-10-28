using System;
using ECommerceInventorySystem.Models;
using ECommerceInventorySystem.Services;
using ECommerceInventorySystem.Warehouse;

namespace ECommerceInventorySystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var warehouse = new Warehouse<Product>
            {
                Location = WarehouseLocation.MainWarehouse,
                ShelfLevels = new Product[10],
            };

            // Beispiel-Produkt hinzufügen
            var product = new Product
            {
                SKU = "ABC123",
                Name = "Monitor",
                Description = "27 Zoll LED",
                Category = ProductCategory.Electronics,
                Condition = ProductCondition.New,
                Price = 199.99m,
                ManufacturedDate = DateTime.Now.AddMonths(-2),
                ExpiryDate = null,
                Supplier = "TechSupplier",
                Quantity = 25
            };

            warehouse.Products.Add(product);
            warehouse.UniqueSKUs.Add(product.SKU);
            warehouse.StockLevels[product.SKU] = new InventoryLevel { Quantity = product.Quantity, LastUpdated = DateTime.Now };

            var manager = new InventoryManager(warehouse.StockLevels);

            manager.AddStock(product.SKU, 5);
            Console.WriteLine(manager.CheckStockLevel(product.SKU));
            Console.WriteLine(product.ToString());
        }
    }
}
