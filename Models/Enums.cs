namespace AutoTrade.Models;

public enum VehicleCondition
{
    New,
    Used
}

public enum BodyType
{
    Any,
    Sedan,
    Hatchback,
    SUV,
    Coupe,
    Universal,
    Minivan
}

public enum VehicleOrigin
{
    Domestic,
    Foreign
}

public enum SortOption
{
    Brand,
    Model,
    Year,
    BasePrice,
    FinalPrice,
    Origin
}
