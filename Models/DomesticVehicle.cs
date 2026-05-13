namespace AutoTrade.Models;

public class DomesticVehicle : Vehicle
{
    public override double CalculateFinalPrice()
    {
        // No import tax for domestic vehicles
        return BasePrice;
    }
}
