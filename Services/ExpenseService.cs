using ExpenseTracker.Cli.Models;
using ExpenseTracker.Cli.Storage;

namespace ExpenseTracker.Cli.Services;

/// <summary>
/// Service for managing expenses and providing summaries.
/// </summary>
public class ExpenseService
{
    private readonly JsonExpenseRepository _storage;
    private readonly List<Expense> _expenses;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseService"/> class.
    /// </summary>
    /// <param name="storage">The storage repository for expenses.</param>
    public ExpenseService(JsonExpenseRepository storage)
    {
        _storage = storage;
        _expenses = _storage.Load();
    }

    /// <summary>
    /// Adds a new expense to the collection and saves it to storage.
    /// </summary>
    /// <param name="description">The description of the expense.</param>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="category">The category of the expense.</param>
    /// <param name="date">The date when the expense occurred.</param>
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

    /// <summary>
    /// Retrieves all expenses ordered by date descending.
    /// </summary>
    /// <returns>A list of all expenses.</returns>
    public List<Expense> GetAllExpenses()
    {
        return [.. _expenses.OrderByDescending(e => e.Date)];
    }

    /// <summary>
    /// Deletes an expense by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the expense to delete.</param>
    /// <returns><c>true</c> if the expense was found and deleted; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Calculates the total amount of all expenses.
    /// </summary>
    /// <returns>The total monetary amount.</returns>
    public decimal GetTotalAmount()
    {
        return _expenses.Sum(e => e.Amount);
    }

    /// <summary>
    /// Gets a summary of expenses grouped by category.
    /// </summary>
    /// <returns>A dictionary where the key is the category and the value is the total amount.</returns>
    public Dictionary<string, decimal> GetSummaryByCategory()
    {
        return _expenses.GroupBy(e => e.Category)
                       .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
    }

    /// <summary>
    /// Calculates the total amount for expenses in a specific month and year.
    /// </summary>
    /// <param name="month">The month (1-12).</param>
    /// <param name="year">The year.</param>
    /// <returns>The total monetary amount for the specified period.</returns>
    public decimal GetTotalAmountByMonth(int month, int year)
    {
        return _expenses.Where(e => e.Date.Month == month && e.Date.Year == year)
                       .Sum(e => e.Amount);
    }
}
