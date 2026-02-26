using System;

namespace ExpenseTracker.Cli.Models
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = "General";
        public DateTime Date { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{Date:yyyy-MM-dd}] {Description} - ${Amount:F2} ({Category}) - ID: {Id}";
        }
    }
}
