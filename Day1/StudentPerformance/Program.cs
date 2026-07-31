using System;

namespace StudentPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Student Performance Calculator ===\n");

            double total = 0;
            int numberOfSubjects = 5;
            double maxMarksPerSubject = 100;

            for (int i = 1; i <= numberOfSubjects; i++)
            {
                Console.Write($"Enter marks for Subject {i} (0-100): ");
                string? input = Console.ReadLine();

                // Validate that the input is a valid number and between 0 and 100
                if (!double.TryParse(input, out double mark) || mark < 0 || mark > 100)
                {
                    Console.WriteLine($"\nError: Invalid input for Subject {i}. Marks must be a numeric value between 0 and 100.");
                    Console.WriteLine("Exiting program...");
                    return; 
                }

                total += mark; // Add valid mark to total
            }

            // Calculate average
            double average = total / numberOfSubjects;
            
            // Calculate percentage (total marks obtained / total maximum marks) * 100
            double percentage = (total / (numberOfSubjects * maxMarksPerSubject)) * 100;
            
            // Round to two decimal places
            percentage = Math.Round(percentage, 2);
            average = Math.Round(average, 2); // Also rounding average for clean output

            Console.WriteLine("\n=== Performance Results ===");
            Console.WriteLine($"Total Marks: {total} / {numberOfSubjects * maxMarksPerSubject}");
            Console.WriteLine($"Average:     {average}");
            Console.WriteLine($"Percentage:  {percentage}%");
        }
    }
}
