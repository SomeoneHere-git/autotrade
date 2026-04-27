using System.Text.Json;
using System.Text.Json.Serialization;

// ENUM брендів
enum Brand
{
    BMW,
    Audi,
    Toyota,
    Ford,
    Tesla
}

// ENUM деталей
enum Part
{
    Engine,
    Transmission,
    Suspension,
    Body,
    Electronics
}

// ENUM стану
enum Condition
{
    New,
    Good,
    Used,
    Broken
}

class CarSelector
{
    public virtual bool IsMatch(Car car, Buyer buyer)
    {
        return car.Price <= buyer.MaxPrice;
    }
}

class AdvancedSelector : CarSelector
{
    public override bool IsMatch(Car car, Buyer buyer)
    {
        if (car.Price > buyer.MaxPrice)
            return false;

        if (car.Brand != buyer.DesiredBrand)
            return false;

        foreach (var desired in buyer.DesiredParts)
        {
            if (car.Parts.ContainsKey(desired.Key))
            {
                if (car.Parts[desired.Key] > desired.Value)
                    return false;
            }
        }

        return true;
    }
}

// АВТОСАЛОН
class AutoSalon
{
    public List<Car> Cars { get; set; } = new List<Car>();

    public List<Car> FindMatches(Buyer buyer, CarSelector selector)
    {
        List<Car> result = new List<Car>();

        foreach (var car in Cars)
        {
            if (selector.IsMatch(car, buyer))
                result.Add(car);
        }

        return result;
    }

    // ЗБЕРЕЖЕННЯ В JSON
    public void SaveToJson(string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            Converters = { new JsonStringEnumConverter() }
        };

        string json = JsonSerializer.Serialize(Cars, options);
        File.WriteAllText(path, json);
    }

    // ЗАВАНТАЖЕННЯ З JSON
    public void LoadFromJson(string path)
    {
        if (!File.Exists(path))
            return;

        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            Converters = { new JsonStringEnumConverter() }
        };

        string json = File.ReadAllText(path);
        Cars = JsonSerializer.Deserialize<List<Car>>(json, options);
    }
}

// ЗАЯВКА
class SupplierRequest
{
    public Brand Brand { get; set; }
    public decimal Budget { get; set; }
    public Dictionary<Part, Condition> RequiredParts { get; set; }

    public override string ToString()
    {
        string parts = "";

        foreach (var p in RequiredParts)
        {
            parts += $"{p.Key}: {p.Value}; ";
        }

        return $"Request -> Brand: {Brand}, Budget: {Budget}, Parts: {parts}";
    }
}


