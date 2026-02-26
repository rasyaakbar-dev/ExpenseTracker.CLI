# Expense Tracker CLI

A modern, high-performance command-line application for managing your daily expenses, built with **.NET 10** and **C# 12+**.

## 🚀 Features

- **Add Expenses**: Quickly record expenses with descriptions, amounts, categories, and dates.
- **List View**: View your expense history in a clean, formatted table.
- **Summaries**:
  - Get a grand total of all spending.
  - Filter summaries by month and year.
  - View spending breakdowns grouped by category.
- **Delete**: Remove entries using their unique identifiers.

## 🛠️ Technical Highlights

- **System.CommandLine**: Utilizes the official .NET library for robust command parsing and help generation.
- **Clean Architecture**: Separation of concerns across Models, Services, and Storage layers.
- **Modern C# Features**: Leverages C# 12+ features such as Primary Constructors, Collection Expressions, and File-Scoped Namespaces.
- **JSON Persistence**: Simple and portable data storage.
- **Coding Standards**: Strictly adheres to the **Official Microsoft C# Coding Conventions**.

## 📖 Usage

### Getting Started

Ensure you have the [.NET 10 SDK](https://dotnet.microsoft.com/download) installed.

```bash
# Clone and build
dotnet build
```

### Commands

#### Add a new expense

```bash
dotnet run -- add --description "Coffee" --amount 4.50 --category "Food"
# Short aliases
dotnet run -- add -d "Lunch" -a 15.00 -c "Food" -t "2026-02-26"
```

#### List all expenses

```bash
dotnet run -- list
```

#### Show summaries

```bash
# Overall summary
dotnet run -- summary

# Monthly summary
dotnet run -- summary --month 2 --year 2026
```

#### Delete an expense

```bash
dotnet run -- delete --id [GUID]
```

## 📝 License

This project is licensed under the **MIT License**. See the [LICENSE](file:///home/rasya/Repository/ExpenseTracker.Cli/LICENSE) file for details.
