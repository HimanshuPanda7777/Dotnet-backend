using System;
using System.Collections.Generic;

namespace MultiLevelApproval
{
    // Expense Request Class
    public class ExpenseRequest
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public ExpenseRequest(int id, string description, decimal amount)
        {
            Id = id;
            Description = description;
            Amount = amount;
        }
    }

    // Abstract Handler
    public abstract class ExpenseApprover
    {
        protected ExpenseApprover _nextApprover;

        public void SetNextApprover(ExpenseApprover nextApprover)
        {
            _nextApprover = nextApprover;
        }

        public abstract void ProcessRequest(ExpenseRequest request);
    }

    // Concrete Handlers
    public class TeamLead : ExpenseApprover
    {
        public override void ProcessRequest(ExpenseRequest request)
        {
            if (request.Amount <= 10000)
            {
                Console.WriteLine($"TeamLead approved expense #{request.Id} for {request.Description} of \u20b9{request.Amount}");
            }
            else if (_nextApprover != null)
            {
                _nextApprover.ProcessRequest(request);
            }
        }
    }

    public class Manager : ExpenseApprover
    {
        public override void ProcessRequest(ExpenseRequest request)
        {
            if (request.Amount <= 50000)
            {
                Console.WriteLine($"Manager approved expense #{request.Id} for {request.Description} of \u20b9{request.Amount}");
            }
            else if (_nextApprover != null)
            {
                _nextApprover.ProcessRequest(request);
            }
        }
    }

    public class Director : ExpenseApprover
    {
        public override void ProcessRequest(ExpenseRequest request)
        {
            Console.WriteLine($"Director approved expense #{request.Id} for {request.Description} of \u20b9{request.Amount}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Set up the Chain of Responsibility
            ExpenseApprover teamLead = new TeamLead();
            ExpenseApprover manager = new Manager();
            ExpenseApprover director = new Director();

            teamLead.SetNextApprover(manager);
            manager.SetNextApprover(director);

            // Create some expense requests
            var requests = new List<ExpenseRequest>
            {
                new ExpenseRequest(1, "Office Supplies", 5000),
                new ExpenseRequest(2, "New Laptops", 35000),
                new ExpenseRequest(3, "Annual Conference Event", 120000)
            };

            Console.WriteLine("--- Expense Processing System ---");
            foreach (var req in requests)
            {
                teamLead.ProcessRequest(req);
            }
        }
    }
}
