using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using AutoTrade.Models;

namespace AutoTrade.Services;

public class AppData
{
    public List<Vehicle> Vehicles { get; set; } = new();
    public List<Customer> Customers { get; set; } = new();
}

public class DataService
{
    private readonly JsonSerializerOptions _options;

    public DataService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    // Synchronous save method
    public void SaveData(string filePath, AppData data)
    {
        var json = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(filePath, json);
    }

    // Synchronous load method
    public AppData LoadData(string filePath)
    {
        if (!File.Exists(filePath))
            return new AppData();

        var json = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<AppData>(json, _options);
        return data ?? new AppData();
    }
}
