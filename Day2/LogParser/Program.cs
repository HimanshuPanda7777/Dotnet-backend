using System;

namespace LogParserApp
{
    public enum LogLevel { Info, Warning, Error }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Log Parser ---");
            string logLine = "2023-10-27 14:30:00 ERROR: Disk full";
            int count = 0;

            ParseLogLine(in logLine, out DateTime ts, out LogLevel level, ref count);

            Console.WriteLine($"Timestamp: {ts}");
            Console.WriteLine($"Level: {level}");
            Console.WriteLine($"Counter: {count}");
            Console.ReadLine();
        }

        public static void ParseLogLine(in string logLine, out DateTime timestamp, out LogLevel level, ref int counter)
        {
            string[] parts = logLine.Split(' ', 3);
            
            if (parts.Length >= 3)
            {
                DateTime.TryParse(parts[0] + " " + parts[1], out timestamp);
                
                string levelString = parts[2].Split(':')[0];
                Enum.TryParse(levelString, true, out level);
                
                counter++;
            }
            else
            {
                timestamp = default;
                level = default;
            }
        }
    }
}
