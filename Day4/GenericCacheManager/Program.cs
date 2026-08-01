using System;
using System.Collections.Generic;
using System.Linq;

namespace GenericCacheManager
{
    // Custom Exception
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(string message) : base(message) { }
    }

    // Cache Item Wrapper to track expiration
    public class CacheItem<T>
    {
        public T Value { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool IsExpired => DateTime.Now > ExpirationTime;
    }

    // Generic Cache Manager Class
    public class CacheManager<T>
    {
        private readonly Dictionary<string, CacheItem<T>> _cache = new Dictionary<string, CacheItem<T>>();

        // Add item
        public void Add(string key, T value, TimeSpan expirationDuration)
        {
            _cache[key] = new CacheItem<T>
            {
                Value = value,
                ExpirationTime = DateTime.Now.Add(expirationDuration)
            };
            Console.WriteLine($"Added '{key}' to cache.");
        }

        // Remove item
        public void Remove(string key)
        {
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
                Console.WriteLine($"Removed '{key}' from cache.");
            }
            else
            {
                throw new InvalidKeyException($"Key '{key}' not found in cache.");
            }
        }

        // Get By Key
        public T GetByKey(string key)
        {
            if (_cache.TryGetValue(key, out var item))
            {
                if (item.IsExpired)
                {
                    _cache.Remove(key);
                    throw new InvalidKeyException($"Key '{key}' has expired.");
                }
                return item.Value;
            }
            throw new InvalidKeyException($"Key '{key}' not found in cache.");
        }

        // Clear all items
        public void Clear()
        {
            _cache.Clear();
            Console.WriteLine("Cache cleared.");
        }

        // Indexer
        public T this[string key]
        {
            get => GetByKey(key);
            set => Add(key, value, TimeSpan.FromMinutes(10)); // Default 10 min
        }
        
        // Expose underlying dictionary for extension methods
        internal Dictionary<string, CacheItem<T>> GetCacheDictionary() => _cache;
    }

    // Extension Methods
    public static class CacheManagerExtensions
    {
        public static List<string> GetAllKeys<T>(this CacheManager<T> cacheManager)
        {
            return cacheManager.GetCacheDictionary().Keys.ToList();
        }

        public static int CountExpiredItems<T>(this CacheManager<T> cacheManager)
        {
            return cacheManager.GetCacheDictionary().Values.Count(item => item.IsExpired);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("--- Generic Cache Manager ---");
                
                var intCache = new CacheManager<int>();
                intCache.Add("UserId", 101, TimeSpan.FromMinutes(5));
                
                var stringCache = new CacheManager<string>();
                stringCache["SessionToken"] = "XYZ12345"; 

                // Test Indexer
                Console.WriteLine($"SessionToken: {stringCache["SessionToken"]}");

                // Simulate expiration
                stringCache.Add("TempData", "WillExpireSoon", TimeSpan.FromMilliseconds(10));
                System.Threading.Thread.Sleep(20); // Wait for expiration

                Console.WriteLine($"Total Keys in string cache: {stringCache.GetAllKeys().Count}");
                Console.WriteLine($"Expired Items in string cache: {stringCache.CountExpiredItems()}");
                
                // This will throw exception
                Console.WriteLine($"TempData: {stringCache["TempData"]}");
            }
            catch (InvalidKeyException ex)
            {
                Console.WriteLine($"Cache Error: {ex.Message}");
            }
        }
    }
}
