# Expense Tracker CLI

A lightweight, high-performance console tool for managing daily expenses, built with **.NET 10** and **C# 12+**.

## Features

- **Add**: Record expenses with description, amount, category, and date.
- **List**: View all expenses in a clean, formatted table.
- **Summary**: Get spending totals and category-based breakdowns.
- **Delete**: Remove records using unique IDs.
- **Async**: Fully asynchronous I/O for performance.

## Tech Stack

- **.NET 10** & **C# 12+** (Top-Level Statements, Primary Constructors, required props).
- **System.CommandLine** for robust parsing.
- **System.Text.Json** for async data persistence.

## Usage

### Commands

```bash
# Add expense
dotnet run -- add -d "Coffee" -a 4.50 -c "Food"

# List all
dotnet run -- list

# Summary (Overall or Monthly)
dotnet run -- summary
dotnet run -- summary -m 2 -y 2026

# Delete
dotnet run -- delete -i [GUID]
```

## License

MIT License. See [LICENSE](file:///home/rasya/Repository/ExpenseTracker.Cli/LICENSE) for details.
