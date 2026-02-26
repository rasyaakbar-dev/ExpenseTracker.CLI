using System;
using System.CommandLine;
using System.Globalization;
using System.Linq;
using ExpenseTracker.Cli.Services;
using ExpenseTracker.Cli.Storage;
using ExpenseTracker.Cli.Utils;

var service = new ExpenseService(new JsonExpenseRepository());
await service.InitializeAsync();

var rootCommand = new RootCommand("Expense Tracker CLI - Manage your expenses easily");

// Add Command
var addCommand = new Command("add", "Add a new expense");
var descOption = new Option<string>("--description", "Description of the expense") { Required = true };
descOption.Aliases.Add("-d");
var amountOption = new Option<decimal>("--amount", "Amount of the expense") { Required = true };
amountOption.Aliases.Add("-a");
var categoryOption = new Option<string>("--category", "Category of the expense");
categoryOption.Aliases.Add("-c");
categoryOption.DefaultValueFactory = _ => "General";
var dateOption = new Option<string>("--date", "Date of the expense (yyyy-MM-dd)");
dateOption.Aliases.Add("-t");
dateOption.DefaultValueFactory = _ => DateTime.Now.ToString("yyyy-MM-dd");

addCommand.Options.Add(descOption);
addCommand.Options.Add(amountOption);
addCommand.Options.Add(categoryOption);
addCommand.Options.Add(dateOption);

addCommand.SetAction(async result =>
{
    var desc = result.GetValue(descOption)!;
    var amount = result.GetValue(amountOption);
    var category = result.GetValue(categoryOption)!;
    var dateStr = result.GetValue(dateOption)!;

    string[] formats = ["yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy"];
    if (!DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
    {
        ConsoleHelper.WriteError($"Invalid date format: {dateStr}. Use yyyy-MM-dd.");
        return;
    }

    await service.AddExpenseAsync(desc, amount, category, date);
    ConsoleHelper.WriteSuccess($"Expense added successfully: {desc} (${amount:F2})");
});

// List Command
var listCommand = new Command("list", "List all expenses");
listCommand.SetAction(_ =>
{
    var expenses = service.GetAllExpenses();
    if (expenses.Count == 0)
    {
        ConsoleHelper.WriteWarning("No expenses found.");
        return;
    }

    ConsoleHelper.WriteInfo($"Showing {expenses.Count} expenses:\n");
    var headers = new[] { "Date", "Description", "Amount", "Category", "ID" };
    var rows = expenses.Select(e => new[] { e.Date.ToString("yyyy-MM-dd"), e.Description, $"${e.Amount:F2}", e.Category, e.Id.ToString() }).ToList();
    ConsoleHelper.WriteTable(headers, rows);
});

// Delete Command
var deleteCommand = new Command("delete", "Delete an expense by ID");
var idOption = new Option<Guid>("--id") { Description = "The ID of the expense to delete", Required = true };
idOption.Aliases.Add("-i");
deleteCommand.Options.Add(idOption);
deleteCommand.SetAction(async result =>
{
    var id = result.GetValue(idOption);
    if (await service.DeleteExpenseAsync(id)) ConsoleHelper.WriteSuccess($"Expense with ID {id} deleted successfully.");
    else ConsoleHelper.WriteError($"Expense with ID {id} not found.");
});

// Summary Command
var summaryCommand = new Command("summary", "Show expense summary");
var monthOption = new Option<int?>("--month") { Description = "Filter by month (1-12)" };
monthOption.Aliases.Add("-m");
var yearOption = new Option<int?>("--year") { Description = "Filter by year" };
yearOption.Aliases.Add("-y");
summaryCommand.Options.Add(monthOption);
summaryCommand.Options.Add(yearOption);
summaryCommand.SetAction(result =>
{
    var month = result.GetValue(monthOption);
    var year = result.GetValue(yearOption) ?? DateTime.Now.Year;

    if (month is < 1 or > 12)
    {
        ConsoleHelper.WriteError("Month must be between 1 and 12.");
        return;
    }

    if (month.HasValue)
    {
        var total = service.GetTotalAmountByMonth(month.Value, year);
        ConsoleHelper.WriteInfo($"Total expenses for {month.Value}/{year}: ${total:F2}");
    }
    else
    {
        ConsoleHelper.WriteInfo($"Total expenses: ${service.GetTotalAmount():F2}\n");
        var summary = service.GetSummaryByCategory();
        if (summary.Count > 0)
        {
            ConsoleHelper.WriteInfo("Summary by category:");
            ConsoleHelper.WriteTable(["Category", "Amount"], summary.Select(s => new[] { s.Key, $"${s.Value:F2}" }).ToList());
        }
    }
});

rootCommand.Subcommands.Add(addCommand);
rootCommand.Subcommands.Add(listCommand);
rootCommand.Subcommands.Add(deleteCommand);
rootCommand.Subcommands.Add(summaryCommand);

return await rootCommand.Parse(args).InvokeAsync();
