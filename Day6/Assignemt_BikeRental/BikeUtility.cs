using System;
using System.Collections.Generic;

namespace Assignemt_BikeRental
{
    public class BikeUtility
    {
        public void AddBikeDetails(string model, string brand, int pricePerDay)
        {
            Bike newBike = new Bike
            {
                Model = model,
                Brand = brand,
                PricePerDay = pricePerDay
            };

            int newKey = Program.bikeDetails.Count + 1;
            Program.bikeDetails.Add(newKey, newBike);
        }

        public SortedDictionary<string, List<Bike>> GroupBikesByBrand()
        {
            SortedDictionary<string, List<Bike>> groupedResult = new SortedDictionary<string, List<Bike>>();

            foreach (var item in Program.bikeDetails)
            {
                Bike currentBike = item.Value;
                
                if (groupedResult.ContainsKey(currentBike.Brand))
                {
                    groupedResult[currentBike.Brand].Add(currentBike);
                }
                else
                {
                    List<Bike> newBikeList = new List<Bike>();
                    newBikeList.Add(currentBike);
                    groupedResult.Add(currentBike.Brand, newBikeList);
                }
            }

            return groupedResult;
        }
    }
}
