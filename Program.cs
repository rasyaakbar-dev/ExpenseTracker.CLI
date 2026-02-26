using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Cli.Services;
using ExpenseTracker.Cli.Storage;
using ExpenseTracker.Cli.Models;

namespace ExpenseTracker.Cli
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var storage = new JsonExpenseRepository();
            var service = new ExpenseService(storage);

            var rootCommand = new RootCommand("Expense Tracker CLI - Manage your expenses easily");

            rootCommand.Subcommands.Add(CreateAddCommand(service));
            rootCommand.Subcommands.Add(CreateListCommand(service));
            rootCommand.Subcommands.Add(CreateDeleteCommand(service));
            rootCommand.Subcommands.Add(CreateSummaryCommand(service));

            return rootCommand.Parse(args).Invoke();
        }

        private static Command CreateAddCommand(ExpenseService service)
        {
            var command = new Command("add", "Add a new expense");

            var descriptionOption = new Option<string>("--description") { Description = "Description of the expense" };
            descriptionOption.Aliases.Add("-d");
            descriptionOption.Required = true;

            var amountOption = new Option<decimal>("--amount") { Description = "Amount of the expense" };
            amountOption.Aliases.Add("-a");
            amountOption.Required = true;

            var categoryOption = new Option<string>("--category") { Description = "Category of the expense" };
            categoryOption.Aliases.Add("-c");
            categoryOption.DefaultValueFactory = _ => "General";

            var dateOption = new Option<string>("--date") { Description = "Date of the expense (yyyy-MM-dd)" };
            dateOption.Aliases.Add("-t");
            dateOption.DefaultValueFactory = _ => DateTime.Now.ToString("yyyy-MM-dd");

            command.Options.Add(descriptionOption);
            command.Options.Add(amountOption);
            command.Options.Add(categoryOption);
            command.Options.Add(dateOption);

            command.SetAction(result =>
            {
                var description = result.GetValue(descriptionOption)!;
                var amount = result.GetValue(amountOption);
                var category = result.GetValue(categoryOption)!;
                var dateStr = result.GetValue(dateOption)!;

                if (!TryParseDate(dateStr, out DateTime date))
                {
                    WriteError($"Invalid date format: {dateStr}. Use yyyy-MM-dd.");
                    return;
                }

                service.AddExpense(description, amount, category, date);
                WriteSuccess($"Expense added successfully: {description} (${amount:F2})");
            });

            return command;
        }

        private static Command CreateListCommand(ExpenseService service)
        {
            var command = new Command("list", "List all expenses");

            command.SetAction(result =>
            {
                var expenses = service.GetAllExpenses();

                if (!expenses.Any())
                {
                    WriteWarning("No expenses found.");
                    return;
                }

                WriteInfo($"Showing {expenses.Count} expenses:");
                Console.WriteLine();

                string[] headers = { "Date", "Description", "Amount", "Category", "ID" };
                var rows = expenses.Select(e => new[]
                {
                    e.Date.ToString("yyyy-MM-dd"),
                    e.Description,
                    $"${e.Amount:F2}",
                    e.Category,
                    e.Id.ToString()
                }).ToList();

                WriteTable(headers, rows);
            });

            return command;
        }

        private static Command CreateDeleteCommand(ExpenseService service)
        {
            var command = new Command("delete", "Delete an expense by ID");

            var idOption = new Option<Guid>("--id") { Description = "The ID of the expense to delete", Required = true };
            idOption.Aliases.Add("-i");

            command.Options.Add(idOption);

            command.SetAction(result =>
            {
                var id = result.GetValue(idOption);
                if (service.DeleteExpense(id))
                {
                    WriteSuccess($"Expense with ID {id} deleted successfully.");
                }
                else
                {
                    WriteError($"Expense with ID {id} not found.");
                }
            });

            return command;
        }

        private static Command CreateSummaryCommand(ExpenseService service)
        {
            var command = new Command("summary", "Show expense summary");

            var monthOption = new Option<int?>("--month") { Description = "Filter by month (1-12)" };
            monthOption.Aliases.Add("-m");

            var yearOption = new Option<int?>("--year") { Description = "Filter by year" };
            yearOption.Aliases.Add("-y");

            command.Options.Add(monthOption);
            command.Options.Add(yearOption);

            command.SetAction(result =>
            {
                var month = result.GetValue(monthOption);
                var year = result.GetValue(yearOption);

                if (month.HasValue && (month < 1 || month > 12))
                {
                    WriteError("Month must be between 1 and 12.");
                    return;
                }

                int currentYear = year ?? DateTime.Now.Year;

                if (month.HasValue)
                {
                    decimal total = service.GetTotalAmountByMonth(month.Value, currentYear);
                    WriteInfo($"Total expenses for {month.Value}/{currentYear}: ${total:F2}");
                }
                else
                {
                    decimal total = service.GetTotalAmount();
                    var summary = service.GetSummaryByCategory();

                    WriteInfo($"Total expenses: ${total:F2}");
                    Console.WriteLine();

                    if (summary.Any())
                    {
                        WriteInfo("Summary by category:");
                        string[] headers = { "Category", "Amount" };
                        var rows = summary.Select(s => new[] { s.Key, $"${s.Value:F2}" }).ToList();
                        WriteTable(headers, rows);
                    }
                }
            });

            return command;
        }

        #region Helpers

        private static bool TryParseDate(string input, out DateTime date)
        {
            string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };
            return DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        private static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void WriteTable(string[] headers, List<string[]> rows)
        {
            int[] columnWidths = new int[headers.Length];
            for (int i = 0; i < headers.Length; i++)
            {
                columnWidths[i] = headers[i].Length;
                foreach (var row in rows)
                {
                    if (row[i].Length > columnWidths[i])
                        columnWidths[i] = row[i].Length;
                }
            }

            for (int i = 0; i < headers.Length; i++)
            {
                Console.Write(headers[i].PadRight(columnWidths[i] + 2));
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', columnWidths.Sum() + (headers.Length * 2)));

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    Console.Write(row[i].PadRight(columnWidths[i] + 2));
                }
                Console.WriteLine();
            }
        }

        #endregion
    }
}
