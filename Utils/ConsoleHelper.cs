using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Cli.Utils;

/// <summary>
/// Provides utility methods for consistent console output.
/// </summary>
public static class ConsoleHelper
{
    /// <summary>Writes a success message in green.</summary>
    public static void WriteSuccess(string message) => WriteColored(message, ConsoleColor.Green);
    /// <summary>Writes an error message in red.</summary>
    public static void WriteError(string message) => WriteColored(message, ConsoleColor.Red);
    /// <summary>Writes an info message in cyan.</summary>
    public static void WriteInfo(string message) => WriteColored(message, ConsoleColor.Cyan);
    /// <summary>Writes a warning message in yellow.</summary>
    public static void WriteWarning(string message) => WriteColored(message, ConsoleColor.Yellow);

    private static void WriteColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    /// <summary>
    /// Renders a table with the specified headers and rows to the console.
    /// </summary>
    /// <param name="headers">The table headers.</param>
    /// <param name="rows">The table rows.</param>
    public static void WriteTable(string[] headers, List<string[]> rows)
    {
        int[] columnWidths = new int[headers.Length];
        for (var i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = Math.Max(headers[i].Length, rows.Count > 0 ? rows.Max(row => row[i].Length) : 0);
        }

        for (var i = 0; i < headers.Length; i++)
        {
            Console.Write(headers[i].PadRight(columnWidths[i] + 2));
        }

        Console.WriteLine();
        Console.WriteLine(new string('-', columnWidths.Sum() + (headers.Length * 2)));

        foreach (var row in rows)
        {
            for (var i = 0; i < row.Length; i++)
            {
                Console.Write(row[i].PadRight(columnWidths[i] + 2));
            }
            Console.WriteLine();
        }
    }
}
