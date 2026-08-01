using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeePayrollEngine
{
    // Abstract Employee Class
    public abstract class Employee
    {
        public int Id { get; set; }
        
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }

        public abstract decimal CalculateSalary();
        public abstract decimal CalculateBonus();
    }

    // Subclasses
    public class PermanentEmployee : Employee
    {
        public decimal BasicPay { get; set; }
        public decimal HRA { get; set; }

        public override decimal CalculateSalary() => BasicPay + HRA;
        
        public override decimal CalculateBonus() => BasicPay * 0.1m; // 10% bonus
    }

    public class ContractEmployee : Employee
    {
        public decimal HourlyRate { get; set; }
        public int HoursWorked { get; set; }

        public override decimal CalculateSalary() => HourlyRate * HoursWorked;
        
        public override decimal CalculateBonus() => 0; // No bonus
    }

    public class Intern : Employee
    {
        public decimal Stipend { get; set; }

        public override decimal CalculateSalary() => Stipend;
        
        public override decimal CalculateBonus() => 500; // Fixed bonus
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Object Initializers
            var employees = new List<Employee>
            {
                new PermanentEmployee { Id = 1, Name = "Pankaj", BasicPay = 50000, HRA = 10000 },
                new ContractEmployee { Id = 2, Name = "Rahul", HourlyRate = 500, HoursWorked = 160 },
                new Intern { Id = 3, Name = "Sneha", Stipend = 15000 }
            };

            // Anonymous Types for reporting
            var payrollReports = employees.Select(e => new
            {
                EmployeeId = e.Id,
                EmployeeName = e.Name,
                Type = e.GetType().Name,
                TotalSalary = e.CalculateSalary(),
                Bonus = e.CalculateBonus(),
                NetPay = e.CalculateSalary() + e.CalculateBonus()
            });

            Console.WriteLine("--- Payroll Report ---");
            foreach (var report in payrollReports)
            {
                Console.WriteLine($"[{report.Type}] {report.EmployeeName} (ID: {report.EmployeeId})");
                Console.WriteLine($"   Base/Salary: \u20b9{report.TotalSalary}, Bonus: \u20b9{report.Bonus}");
                Console.WriteLine($"   Net Pay: \u20b9{report.NetPay}\n");
            }
        }
    }
}
