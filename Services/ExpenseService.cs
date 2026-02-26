using System;
using System.Collections.Generic;
using System.Linq;
using ExpenseTracker.Cli.Models;
using ExpenseTracker.Cli.Storage;

namespace ExpenseTracker.Cli.Services
{
    public class ExpenseService
    {
        private readonly JsonExpenseRepository _storage;
        private List<Expense> _expenses;

        public ExpenseService(JsonExpenseRepository storage)
        {
            _storage = storage;
            _expenses = _storage.Load();
        }

        public void AddExpense(string description, decimal amount, string category, DateTime date)
        {
            var expense = new Expense
            {
                Description = description,
                Amount = amount,
                Category = category,
                Date = date
            };
            _expenses.Add(expense);
            _storage.Save(_expenses);
        }

        public List<Expense> GetAllExpenses()
        {
            return _expenses.OrderByDescending(e => e.Date).ToList();
        }

        public bool DeleteExpense(Guid id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                _expenses.Remove(expense);
                _storage.Save(_expenses);
                return true;
            }
            return false;
        }

        public decimal GetTotalAmount()
        {
            return _expenses.Sum(e => e.Amount);
        }

        public Dictionary<string, decimal> GetSummaryByCategory()
        {
            return _expenses.GroupBy(e => e.Category)
                           .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        public decimal GetTotalAmountByMonth(int month, int year)
        {
            return _expenses.Where(e => e.Date.Month == month && e.Date.Year == year)
                           .Sum(e => e.Amount);
        }
    }
}
