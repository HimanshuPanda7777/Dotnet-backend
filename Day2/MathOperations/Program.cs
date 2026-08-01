using System;

namespace MathOperationsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Math Operations ---");
            Console.WriteLine($"Add 2 numbers: {Add(5, 10)}");
            Console.WriteLine($"Add multiple: {Add(1, 2, 3, 4, 5)}");
            Console.WriteLine($"Multiply 2 numbers: {Multiply(2, 3)}");
            Console.WriteLine($"Multiply multiple: {Multiply(2, 3, 4, 5)}");
            Console.ReadLine();
        }

        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static int Add(params int[] numbers)
        {
            int sum = 0;
            foreach (int num in numbers) sum += num;
            return sum;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public static int Multiply(params int[] numbers)
        {
            int product = 1;
            foreach (int num in numbers) product *= num;
            return product;
        }
    }
}
