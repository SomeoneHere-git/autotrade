namespace AutoTrade.Models;

public class ForeignVehicle : Vehicle
{
    public double ImportTaxRate { get; set; } = 0.20; // 20%
    public double CustomFees { get; set; } = 500.0;

    public override double CalculateFinalPrice()
    {
        return BasePrice + (BasePrice * ImportTaxRate) + CustomFees;
    }
}
