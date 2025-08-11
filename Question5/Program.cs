using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable record implementing the marker interface
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            using var writer = new StreamWriter(_filePath, false);
            writer.Write(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File {_filePath} does not exist.");
                _log.Clear();
                return;
            }

            using var reader = new StreamReader(_filePath);
            string json = reader.ReadToEnd();
            var items = JsonSerializer.Deserialize<List<T>>(json);
            _log = items ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
            _log.Clear();
        }
    }
}

// Integration Layer
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 5, DateTime.Now.AddDays(-10)));
        _logger.Add(new InventoryItem(2, "Mouse", 20, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(3, "Keyboard", 15, DateTime.Now.AddDays(-3)));
        _logger.Add(new InventoryItem(4, "Monitor", 7, DateTime.Now.AddDays(-7)));
        _logger.Add(new InventoryItem(5, "USB Cable", 50, DateTime.Now.AddDays(-1)));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No inventory items found.");
            return;
        }

        Console.WriteLine("Inventory Items:");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Qty: {item.Quantity}, Added: {item.DateAdded:yyyy-MM-dd}");
        }
    }
}

// Main Program
public static class Program
{
    public static void Main()
    {
        string filePath = "inventory.json";

        var app = new InventoryApp(filePath);

        // Seed data and save
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session
        Console.WriteLine("Clearing data from memory to simulate new session...\n");

        // Load data and print
        app.LoadData();
        app.PrintAllItems();
    }
}
