using System;

namespace UtilityBillingCalculator
{
    // 1. Interface for extensibility
    public interface IBillingCalculator
    {
        double CalculateBill(double unitsConsumed, double rate, double fixedCharges);
    }

    // 2. Residential Implementation
    public class ResidentialBillingCalculator : IBillingCalculator
    {
        public double CalculateBill(double unitsConsumed, double rate, double fixedCharges)
        {
            // Residential customers get a standard calculation + 5% residential tax
            double energyCost = unitsConsumed * rate;
            double tax = energyCost * 0.05;
            return energyCost + fixedCharges + tax;
        }
    }

    // 3. Commercial Implementation
    public class CommercialBillingCalculator : IBillingCalculator
    {
        public double CalculateBill(double unitsConsumed, double rate, double fixedCharges)
        {
            // Commercial customers incur a heavier 15% surcharge due to commercial power grid usage
            double energyCost = unitsConsumed * rate;
            double surcharge = energyCost * 0.15;
            return energyCost + fixedCharges + surcharge;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Utility Billing Calculator ===\n");

            // --- 1. Customer Type Selection with Graceful Validation ---
            IBillingCalculator? calculator = null;
            string customerType = "";
            
            while (calculator == null)
            {
                Console.WriteLine("Select Customer Type:");
                Console.WriteLine("1. Residential");
                Console.WriteLine("2. Commercial");
                Console.Write("Choice (1 or 2): ");
                
                string? choice = Console.ReadLine();
                if (choice == "1")
                {
                    calculator = new ResidentialBillingCalculator();
                    customerType = "Residential";
                }
                else if (choice == "2")
                {
                    calculator = new CommercialBillingCalculator();
                    customerType = "Commercial";
                }
                else
                {
                    Console.WriteLine("Error: Invalid choice. Please enter 1 or 2.\n");
                }
            }

            // --- 2. Input and Validation (Graceful loops ensure app doesn't terminate on invalid data) ---
            double units = GetValidInput("\nEnter Units Consumed (kWh): ");
            double rate = GetValidInput("Enter Rate per Unit ($): ");
            double fixedCharges = GetValidInput("Enter Fixed Charges ($): ");

            // --- 3. Bill Calculation via Interface ---
            double totalBill = calculator.CalculateBill(units, rate, fixedCharges);
            totalBill = Math.Round(totalBill, 2);

            // --- 4. Presentation ---
            Console.WriteLine("\n=== Billing Summary ===");
            Console.WriteLine($"Customer Type:  {customerType}");
            Console.WriteLine($"Units Consumed: {units} kWh");
            Console.WriteLine($"Rate per Unit:  ${rate}");
            Console.WriteLine($"Fixed Charges:  ${fixedCharges}");
            Console.WriteLine($"Total Bill:     ${totalBill}");
        }

        // Helper method to gracefully prompt until valid numeric input is provided
        static double GetValidInput(string prompt)
        {
            double value;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Validation: Must be a number (TryParse) and must not be negative
                if (double.TryParse(input, out value) && value >= 0)
                {
                    break;
                }
                
                Console.WriteLine("Error: Invalid input. Please enter a valid non-negative numeric value.\n");
            }
            return value;
        }
    }
}
