using System;
using System.Collections.Generic;
using ECommerceInventorySystem.Models;
using ECommerceInventorySystem.Warehouse;

namespace ECommerceInventorySystem.Utils
{
    public static class WarehouseSeeder
    {
        private static Random rnd = new Random();

        // Produktnamen & Lieferanten-Dummies
        private static string[] productNames = { "Monitor", "Laptop", "Headset", "Shirt", "Sneaker", "Coffee", "Book", "Lamp" };
        private static string[] suppliers = { "TechSupplier", "SportsInc", "BookMarket", "HomeStore", "FreshFood" };

        // Erzeuge Produkte mit Zufallswerten
        public static void SeedProducts(Warehouse<Product> warehouse, int productCount)
        {
            warehouse.Products.Clear();
            warehouse.UniqueSKUs.Clear();
            warehouse.StockLevels.Clear();

            for (int i = 0; i < productCount; i++)
            {
                var name = productNames[rnd.Next(productNames.Length)];
                var sku = $"SKU{rnd.Next(1000, 9999)}-{rnd.Next(100, 999)}";
                var category = (ProductCategory)rnd.Next(Enum.GetNames(typeof(ProductCategory)).Length);
                var condition = (ProductCondition)rnd.Next(Enum.GetNames(typeof(ProductCondition)).Length);
                var supplier = suppliers[rnd.Next(suppliers.Length)];
                var minStock = rnd.Next(5, 20);
                var currentStock = rnd.Next(0, 40);
                var price = Math.Round(Math.Round((decimal)rnd.Next(10, 150), 2) * (decimal)rnd.NextDouble() * 2 + 10, 2);
                var expiry = category == ProductCategory.Food
                           ? (DateTime?)DateTime.Now.AddDays(rnd.Next(1, 90))
                           : null;

                var product = new Product
                {
                    SKU = sku,
                    Name = name,
                    Description = $"{name} Beschreibung {i}",
                    Category = category,
                    Condition = condition,
                    Supplier = supplier,
                    MinimumStock = minStock,
                    Quantity = currentStock,
                    Price = price,
                    ManufacturedDate = DateTime.Now.AddMonths(-rnd.Next(1, 12)),
                    ExpiryDate = expiry
                };

                warehouse.Products.Add(product);
                warehouse.UniqueSKUs.Add(product.SKU);
                warehouse.StockLevels[product.SKU] = new InventoryLevel { Quantity = currentStock, LastUpdated = DateTime.Now };
            }
        }

        // Erzeuge zufällige Verkaufsdaten
        public static void SeedSalesHistory(Warehouse<Product> warehouse, int salesCount)
        {
            warehouse.SalesHistory.Clear();

            for (int i = 0; i < salesCount; i++)
            {
                if (warehouse.Products.Count == 0) break;
                var product = warehouse.Products[rnd.Next(warehouse.Products.Count)];
                var amount = rnd.Next(1, 5);
                var saleDate = DateTime.Now.AddDays(-rnd.Next(1, 180));
                var salePrice = product.Price;
                var location = WarehouseLocation.MainWarehouse;

                var sale = new SalesTransaction
                {
                    SKU = product.SKU,
                    Quantity = amount,
                    SoldDate = saleDate,
                    SalePrice = salePrice,
                    Location = location
                };

                warehouse.SalesHistory.Add(sale);
            }
        }
    }
}
