using System.Text.Json;
using ExpenseTracker.Cli.Models;

namespace ExpenseTracker.Cli.Storage;

/// <summary>
/// Repository for managing expense storage in a JSON file.
/// </summary>
/// <param name="fileName">The name of the JSON file.</param>
public class JsonExpenseRepository(string fileName = "expenses.json")
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

    /// <summary>
    /// Loads expenses from the JSON file.
    /// </summary>
    /// <returns>A list of expenses. Returns an empty list if the file does not exist or an error occurs.</returns>
    public List<Expense> Load()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Expense>>(json) ?? [];
        }
        catch (JsonException ex)
        {
            // Log or handle specific JSON issues
            Console.WriteLine($"Error parsing expenses: {ex.Message}");
            return [];
        }
        catch (IOException ex)
        {
            // Log or handle file access issues
            Console.WriteLine($"Error reading expenses file: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// Saves the provided expenses to the JSON file.
    /// </summary>
    /// <param name="expenses">The list of expenses to save.</param>
    public void Save(List<Expense> expenses)
    {
        string json = JsonSerializer.Serialize(expenses, SerializerOptions);
        File.WriteAllText(_filePath, json);
    }
}
