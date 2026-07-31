using System;

namespace InvestmentCalculator
{
    // 1. Common interface for extensibility
    public interface IInvestmentCalculator
    {
        double CalculateProjectedValue(double principal, double annualRate, int years);
    }

    // 2. Concrete implementation: Simple Interest
    public class SimpleInterestCalculator : IInvestmentCalculator
    {
        public double CalculateProjectedValue(double principal, double annualRate, int years)
        {
            // Simple Interest Formula: A = P(1 + rt)
            double rateFraction = annualRate / 100;
            return principal * (1 + (rateFraction * years));
        }
    }

    // 3. Concrete implementation: Compound Interest (Annual)
    public class CompoundInterestCalculator : IInvestmentCalculator
    {
        public double CalculateProjectedValue(double principal, double annualRate, int years)
        {
            // Compound Interest Formula: A = P(1 + r)^t
            double rateFraction = annualRate / 100;
            return principal * Math.Pow(1 + rateFraction, years);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Financial Analysis Tool ===\n");

            // --- 1. Select Investment Type ---
            Console.WriteLine("Select Investment Type:");
            Console.WriteLine("1. Simple Interest");
            Console.WriteLine("2. Compound Interest");
            Console.Write("Choice (1 or 2): ");
            
            string? choiceInput = Console.ReadLine();
            IInvestmentCalculator calculator;

            // Extensible design allows easily adding new calculators (e.g. choice 3 for continuous compound)
            if (choiceInput == "1")
            {
                calculator = new SimpleInterestCalculator();
            }
            else if (choiceInput == "2")
            {
                calculator = new CompoundInterestCalculator();
            }
            else
            {
                Console.WriteLine("Error: Invalid choice selected. Exiting...");
                return;
            }

            // --- 2. Input and Validate Principal Amount ---
            Console.Write("\nEnter Principal Amount: $");
            string? principalInput = Console.ReadLine();
            
            // Reject non-numeric, zero, or negative principal
            if (!double.TryParse(principalInput, out double principal) || principal <= 0)
            {
                Console.WriteLine("Error: Principal must be a valid positive number greater than zero.");
                return;
            }

            // --- 3. Input and Validate Annual Rate ---
            Console.Write("Enter Annual Rate (%): ");
            string? rateInput = Console.ReadLine();
            
            // Reject non-numeric, negative, or absurdly high rates (> 200%)
            if (!double.TryParse(rateInput, out double annualRate) || annualRate < 0 || annualRate > 200)
            {
                Console.WriteLine("Error: Annual Rate must be a valid number between 0 and 200.");
                return;
            }

            // --- 4. Input and Validate Duration (Years) ---
            Console.Write("Enter Duration (Years): ");
            string? durationInput = Console.ReadLine();
            
            // Reject non-integer, zero, negative, or extremely high durations
            if (!int.TryParse(durationInput, out int years) || years <= 0 || years > 100)
            {
                Console.WriteLine("Error: Duration must be a valid positive integer (max 100 years).");
                return;
            }

            // --- 5. Calculation ---
            double projectedValue = calculator.CalculateProjectedValue(principal, annualRate, years);

            // --- 6. Rounding and Output ---
            projectedValue = Math.Round(projectedValue, 2);

            Console.WriteLine("\n=== Projection Results ===");
            Console.WriteLine($"Investment Type:   {(choiceInput == "1" ? "Simple Interest" : "Compound Interest")}");
            Console.WriteLine($"Principal:         ${principal}");
            Console.WriteLine($"Annual Rate:       {annualRate}%");
            Console.WriteLine($"Duration:          {years} Years");
            Console.WriteLine($"Projected Value:   ${projectedValue}");
        }
    }
}
