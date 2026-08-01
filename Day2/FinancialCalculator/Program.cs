using System;

namespace FinancialCalculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Financial Calculator ---");
            Console.WriteLine($"Default parameters (annually): {CalculateCompoundInterest(10000, 0.05)}");
            Console.WriteLine($"Named argument for monthly: {CalculateCompoundInterest(10000, 0.05, 10, compoundingFrequency: 12)}");
            Console.ReadLine();
        }

        public static double CalculateCompoundInterest(double principal, double rate, int time = 10, int compoundingFrequency = 1)
        {
            double amount = principal * Math.Pow(1 + (rate / compoundingFrequency), compoundingFrequency * time);
            return Math.Round(amount, 2);
        }
    }
}
