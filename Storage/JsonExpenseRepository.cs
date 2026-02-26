using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ExpenseTracker.Cli.Models;

namespace ExpenseTracker.Cli.Storage
{
    public class JsonExpenseRepository
    {
        private readonly string _filePath;

        public JsonExpenseRepository(string fileName = "expenses.json")
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public List<Expense> Load()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Expense>();
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
            }
            catch
            {
                return new List<Expense>();
            }
        }

        public void Save(List<Expense> expenses)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(expenses, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
