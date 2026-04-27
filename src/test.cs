class Program
{
    static void Main()
    {
        var salon = new AutoSalon();

        var parts1 = new Dictionary<Part, Condition>
        {
            { Part.Engine, Condition.Good },
            { Part.Body, Condition.Good },
            { Part.Electronics, Condition.Used }
        };

        var parts2 = new Dictionary<Part, Condition>
        {
            { Part.Engine, Condition.New },
            { Part.Body, Condition.New },
            { Part.Electronics, Condition.New }
        };

        salon.Cars.Add(new UsedCar
        {
            Brand = Brand.BMW,
            Year = 2015,
            Price = 15000,
            Mileage = 120000,
            Parts = parts1
        });

        salon.Cars.Add(new NewCar
        {
            Brand = Brand.BMW,
            Year = 2023,
            Price = 30000,
            WarrantyYears = 3,
            Parts = parts2
        });

        // ЗБЕРЕГТИ
        salon.SaveToJson("cars.json");

        // ОЧИСТИТИ І ЗАВАНТАЖИТИ
        salon.Cars.Clear();
        salon.LoadFromJson("cars.json");

        var buyer = new Buyer
        {
            Name = "Ivan",
            DesiredBrand = Brand.BMW,
            MaxPrice = 20000,
            DesiredParts = new Dictionary<Part, Condition>
            {
                { Part.Engine, Condition.Good },
                { Part.Body, Condition.Used }
            }
        };

        var selector = new AdvancedSelector();
        var matches = salon.FindMatches(buyer, selector);

        Console.WriteLine("Matched cars:");

        foreach (var car in matches)
        {
            Console.WriteLine(car.GetInfo());
        }

        if (matches.Count == 0)
        {
            var request = new SupplierRequest
            {
                Brand = buyer.DesiredBrand,
                Budget = buyer.MaxPrice,
                RequiredParts = buyer.DesiredParts
            };

            Console.WriteLine(request.ToString());
        }
    }
}
