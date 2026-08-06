using System;
using System.Collections.Generic;

namespace Assignemt_BikeRental
{
    public class Program
    {
        public static SortedDictionary<int, Bike> bikeDetails = new SortedDictionary<int, Bike>();

        static void Main(string[] args)
        {
            BikeUtility utility = new BikeUtility();
            int choice = 0;

            do
            {
                Console.WriteLine(" 1. Add Bike Details");
                Console.WriteLine(" 2. Group Bikes By Brand");
                Console.WriteLine(" 3. Exit");
                
                Console.Write("\n Enter your choice ");
                string input = Console.ReadLine();
                
                if (!int.TryParse(input, out choice))
                {
                    continue;
                }

                if (choice == 1)
                {
                    Console.WriteLine();
                    Console.Write(" Enter the model: ");
                    string model = Console.ReadLine();
                    
                    Console.Write(" Enter the brand: ");
                    string brand = Console.ReadLine();
                    
                    Console.Write(" Enter the price per day: ");
                    int price = int.Parse(Console.ReadLine());

                    utility.AddBikeDetails(model, brand, price);
                    
                    Console.WriteLine("\n Bike details added successfully\n");
                }
                else if (choice == 2)
                {
                    Console.WriteLine();
                    SortedDictionary<string, List<Bike>> groupedBikes = utility.GroupBikesByBrand();

                    foreach (var brandGroup in groupedBikes)
                    {
                        foreach (var bike in brandGroup.Value)
                        {
                            Console.WriteLine($" {bike.Brand} {bike.Model}");
                        }
                    }
                    Console.WriteLine();
                }

            } while (choice != 3);
        }
    }
}
