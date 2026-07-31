using System;

namespace BmiCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Body Mass Index (BMI) Calculator ===\n");

            // Input and validate Weight
            Console.Write("Enter your weight in kilograms (kg): ");
            string? weightInput = Console.ReadLine();
            
            if (!double.TryParse(weightInput, out double weight) || weight <= 0)
            {
                Console.WriteLine("Error: Invalid weight. Please enter a positive numeric value.");
                return;
            }

            // Input and validate Height
            Console.Write("Enter your height in meters (m): ");
            string? heightInput = Console.ReadLine();

            // Height cannot be zero or negative
            if (!double.TryParse(heightInput, out double height) || height <= 0)
            {
                Console.WriteLine("Error: Invalid height. Please enter a positive numeric value greater than zero.");
                return;
            }

            // Calculate BMI: weight (kg) / (height (m) * height (m))
            double bmi = weight / (height * height);
            
            // Round to two decimals
            bmi = Math.Round(bmi, 2);

            Console.WriteLine($"\nYour BMI is: {bmi}");

            // Determine BMI Category
            string category;
            if (bmi < 18.5)
            {
                category = "Underweight";
            }
            else if (bmi < 25)
            {
                category = "Normal weight";
            }
            else if (bmi < 30)
            {
                category = "Overweight";
            }
            else
            {
                category = "Obese";
            }

            Console.WriteLine($"Category: {category}");
        }
    }
}
