using System;
using System.Diagnostics.CodeAnalysis;

namespace ExpenseTracker.Cli.Models;

/// <summary>
/// Represents an expense entry in the tracker.
/// </summary>
public class Expense
{
    /// <summary>
    /// Gets the unique identifier for the expense.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the description of the expense.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the monetary amount of the expense.
    /// </summary>
    public required decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the category of the expense.
    /// </summary>
    public string Category { get; set; } = "General";

    /// <summary>
    /// Gets or sets the date and time when the expense occurred.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.Now;

    /// <summary>
    /// Initializes a new instance of the <see cref="Expense"/> class.
    /// </summary>
    public Expense() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Expense"/> class with specified values.
    /// </summary>
    [SetsRequiredMembers]
    public Expense(string description, decimal amount, string category = "General", DateTime? date = null)
    {
        Description = description;
        Amount = amount;
        Category = category;
        Date = date ?? DateTime.Now;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Date:yyyy-MM-dd}] {Description} - ${Amount:F2} ({Category}) - ID: {Id}";
    }
}
