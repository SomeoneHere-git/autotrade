using System.Collections.Generic;
using System.Linq;
using AutoTrade.Models;

namespace AutoTrade.Services;

public class MatchingEngine
{
    public IEnumerable<Vehicle> FindMatches(IEnumerable<Vehicle> vehicles, CustomerRequirements requirements)
    {
        return vehicles
            .Where(v => v.Matches(requirements))
            .OrderByDescending(v => v.CalculateMatchScore(requirements));
    }
}
