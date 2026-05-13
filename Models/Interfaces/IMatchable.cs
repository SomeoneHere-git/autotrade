namespace AutoTrade.Models.Interfaces;

public interface IMatchable
{
    bool Matches(CustomerRequirements requirements);
    double CalculateMatchScore(CustomerRequirements requirements);
}
