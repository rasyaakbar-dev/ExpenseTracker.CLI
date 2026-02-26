namespace ExpenseTracker.Cli.Models;

/// <summary>
/// Represents an expense entry in the tracker.
/// </summary>
public class Expense
{
    /// <summary>
    /// Gets or sets the unique identifier for the expense.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the description of the expense.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the monetary amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the category of the expense.
    /// </summary>
    public string Category { get; set; } = "General";

    /// <summary>
    /// Gets or sets the date and time when the expense occurred.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.Now;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Date:yyyy-MM-dd}] {Description} - ${Amount:F2} ({Category}) - ID: {Id}";
    }
}
