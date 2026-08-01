using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartEngine
{
    // Product class
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    // Generic Cart
    public class ShoppingCart<T> where T : Product
    {
        private List<T> _items = new List<T>();

        // Add Item
        public void AddItem(T item)
        {
            _items.Add(item);
            Console.WriteLine($"Added {item.Name} to cart.");
        }

        // Remove Item
        public void RemoveItem(T item)
        {
            _items.Remove(item);
            Console.WriteLine($"Removed {item.Name} from cart.");
        }

        // Total Price
        public decimal TotalPrice()
        {
            return _items.Sum(item => item.Price);
        }

        // Indexer
        public T this[int index]
        {
            get
            {
                if (index >= 0 && index < _items.Count)
                    return _items[index];
                throw new IndexOutOfRangeException("Invalid cart item index.");
            }
        }

        // For extension method usage
        public IEnumerable<T> GetItems() => _items;
        public int Count => _items.Count;
    }

    // Extension Methods
    public static class ShoppingCartExtensions
    {
        // Apply percentage discount
        public static decimal ApplyDiscount<T>(this ShoppingCart<T> cart, decimal percentage) where T : Product
        {
            decimal total = cart.TotalPrice();
            return total * (percentage / 100m);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cart = new ShoppingCart<Product>();
            
            cart.AddItem(new Product { Name = "Laptop", Price = 50000, Category = "Electronics" });
            cart.AddItem(new Product { Name = "Headphones", Price = 3000, Category = "Electronics" });
            cart.AddItem(new Product { Name = "Mouse", Price = 1500, Category = "Electronics" });

            // Using Indexer
            Console.WriteLine($"\nFirst item in cart: {cart[0].Name}\n");

            decimal total = cart.TotalPrice();
            decimal discount = cart.ApplyDiscount(10); // 10% discount

            // Anonymous Types to generate invoice summary
            var invoiceSummary = new
            {
                ItemCount = cart.Count,
                Total = total,
                Discount = discount,
                FinalPayable = total - discount
            };

            Console.WriteLine("--- Invoice Summary ---");
            Console.WriteLine("{\n" +
                              $"    ItemCount = {invoiceSummary.ItemCount},\n" +
                              $"    Total = {invoiceSummary.Total},\n" +
                              $"    Discount = {invoiceSummary.Discount},\n" +
                              $"    FinalPayable = {invoiceSummary.FinalPayable}\n" +
                              "}");
        }
    }
}
