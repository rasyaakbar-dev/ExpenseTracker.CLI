using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ExpenseTracker.Cli.Models;

namespace ExpenseTracker.Cli.Storage;

/// <summary>
/// Repository for managing expense storage in a JSON file using asynchronous operations.
/// </summary>
/// <param name="fileName">The name of the JSON file.</param>
public sealed class JsonExpenseRepository(string fileName = "expenses.json")
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

    /// <summary>
    /// Loads expenses from the JSON file asynchronously.
    /// </summary>
    /// <returns>A list of expenses. Returns an empty list if the file does not exist or an error occurs.</returns>
    public async Task<List<Expense>> LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            await using FileStream stream = File.OpenRead(_filePath);
            return await JsonSerializer.DeserializeAsync<List<Expense>>(stream, SerializerOptions) ?? [];
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing expenses: {ex.Message}");
            return [];
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading expenses file: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// Saves the provided expenses to the JSON file asynchronously.
    /// </summary>
    /// <param name="expenses">The list of expenses to save.</param>
    public async Task SaveAsync(List<Expense> expenses)
    {
        await using FileStream stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, expenses, SerializerOptions);
    }
}
