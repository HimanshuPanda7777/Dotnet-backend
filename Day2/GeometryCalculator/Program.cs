using System;

namespace GeometryCalculatorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Geometry Area Calculator ---");
            Console.WriteLine($"Circle area (default precision): {CalculateArea(5)}");
            Console.WriteLine($"Rectangle area: {CalculateArea(4, 6)}");
            Console.WriteLine($"Triangle area: {CalculateArea(3.0, 7.0)}");
            Console.WriteLine($"Circle area (4 decimals): {CalculateArea(radius: 5, decimals: 4)}");
            Console.ReadLine();
        }

        public static double CalculateArea(double radius, int decimals = 2)
        {
            double area = Math.PI * radius * radius;
            return Math.Round(area, decimals);
        }

        public static double CalculateArea(int length, int width)
        {
            return length * width;
        }
        
        public static double CalculateArea(double triangleBase, double height) 
        {
            return 0.5 * triangleBase * height;
        }
    }
}
