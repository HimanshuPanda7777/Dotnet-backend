using System;
using System.Collections.Generic;

namespace OrderProcessorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Order Processor ---");
            string input = "978-3-16-148410-0, 1234567890123, invalid-isbn, 978-1-4028-9462-6";
            
            if (TryProcessOrder(out List<string> validList, input))
            {
                Console.WriteLine("Processed successfully. Valid ISBNs:");
                foreach(var isbn in validList)
                {
                    Console.WriteLine(isbn);
                }
            }
            Console.ReadLine();
        }

        public static bool TryProcessOrder(out List<string> validIsbns, params string[] isbns)
        {
            validIsbns = new List<string>();
            
            foreach (var item in isbns)
            {
                var splitItems = item.Split(','); 
                
                foreach(var isbn in splitItems)
                {
                    if (TryParseISBN(isbn, out string cleanIsbn))
                    {
                        validIsbns.Add(cleanIsbn);
                    }
                }
            }
            
            return validIsbns.Count > 0;
        }

        private static bool TryParseISBN(string input, out string cleaned)
        {
            cleaned = input.Replace("-", "").Trim();
            if (cleaned.Length == 13)
            {
                return true;
            }
            cleaned = string.Empty;
            return false;
        }
    }
}
