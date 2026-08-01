using System;
using System.Collections.Generic;

namespace TrainingManagement
{
    class TrainingProgram
    {
        public int Id { get; set; }
        public string Course { get; set; }
        public string Topic { get; set; }
        public string Provider { get; set; }
        public DateTime ScheduledDate { get; set; }
    }

    class TrainingManager
    {
        private List<TrainingProgram> programs = new List<TrainingProgram>();

        public void AddProgram(TrainingProgram tp)
        {
            programs.Add(tp);
        }

        // List the training program being offered by a training provider
        public List<TrainingProgram> GetProgramsByProvider(string provider)
        {
            List<TrainingProgram> result = new List<TrainingProgram>();
            foreach (var p in programs)
            {
                if (p.Provider == provider)
                    result.Add(p);
            }
            return result;
        }

        // List all the training program scheduled on a day
        public List<TrainingProgram> GetProgramsByDate(DateTime date)
        {
            List<TrainingProgram> result = new List<TrainingProgram>();
            foreach (var p in programs)
            {
                if (p.ScheduledDate.Date == date.Date)
                    result.Add(p);
            }
            return result;
        }

        // List all the training program schedule for a particular course
        public List<TrainingProgram> GetProgramsByCourse(string course)
        {
            List<TrainingProgram> result = new List<TrainingProgram>();
            foreach (var p in programs)
            {
                if (p.Course == course)
                    result.Add(p);
            }
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TrainingManager manager = new TrainingManager();
            manager.AddProgram(new TrainingProgram { Id = 1, Course = "Java", Topic = "OOP", Provider = "TCS", ScheduledDate = new DateTime(2023, 10, 15) });
            manager.AddProgram(new TrainingProgram { Id = 2, Course = "C#", Topic = "LINQ", Provider = "Infosys", ScheduledDate = new DateTime(2023, 10, 15) });
            manager.AddProgram(new TrainingProgram { Id = 3, Course = "Java", Topic = "Collections", Provider = "TCS", ScheduledDate = new DateTime(2023, 10, 20) });

            Console.WriteLine("Programs by provider 'TCS':");
            foreach(var p in manager.GetProgramsByProvider("TCS"))
                Console.WriteLine($"- {p.Course} ({p.Topic})");

            Console.WriteLine("\nPrograms scheduled on 15 Oct 2023:");
            foreach(var p in manager.GetProgramsByDate(new DateTime(2023, 10, 15)))
                Console.WriteLine($"- {p.Course} ({p.Topic}) by {p.Provider}");

            Console.WriteLine("\nPrograms for course 'Java':");
            foreach(var p in manager.GetProgramsByCourse("Java"))
                Console.WriteLine($"- {p.Topic} on {p.ScheduledDate.ToShortDateString()}");
        }
    }
}
