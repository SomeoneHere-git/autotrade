
class Car
{
    public Brand Brand { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }

    public Dictionary<Part, Condition> Parts { get; set; }

    public virtual string GetInfo()
    {
        string partsInfo = "";

        foreach (var part in Parts)
        {
            partsInfo += $"{part.Key}: {part.Value}; ";
        }

        return $"{Brand} ({Year}) - {Price}$ | {partsInfo}";
    }
}

// НАСЛІДУВАННЯ
class UsedCar : Car
{
    public int Mileage { get; set; }

    public override string GetInfo()
    {
        return base.GetInfo() + $" Mileage: {Mileage}";
    }
}

class NewCar : Car
{
    public int WarrantyYears { get; set; }

    public override string GetInfo()
    {
        return base.GetInfo() + $" Warranty: {WarrantyYears} years";
    }
}


