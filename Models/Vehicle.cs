using System;
using System.Text.Json.Serialization;
using AutoTrade.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoTrade.Models;

[JsonDerivedType(typeof(DomesticVehicle), typeDiscriminator: "domestic")]
[JsonDerivedType(typeof(ForeignVehicle), typeDiscriminator: "foreign")]
public abstract class Vehicle : ObservableObject, IStorable, IMatchable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _brand = string.Empty;
    public string Brand
    {
        get => _brand;
        set => SetProperty(ref _brand, value);
    }

    private string _model = string.Empty;
    public string Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    private int _year = DateTime.Now.Year;
    public int Year
    {
        get => _year;
        set => SetProperty(ref _year, value);
    }

    private BodyType _bodyType = BodyType.Sedan;
    public BodyType BodyType
    {
        get => _bodyType;
        set => SetProperty(ref _bodyType, value);
    }

    private string _engine = string.Empty;
    public string Engine
    {
        get => _engine;
        set => SetProperty(ref _engine, value);
    }

    private string _transmission = string.Empty;
    public string Transmission
    {
        get => _transmission;
        set => SetProperty(ref _transmission, value);
    }

    private VehicleCondition _condition;
    public VehicleCondition Condition
    {
        get => _condition;
        set => SetProperty(ref _condition, value);
    }

    private double _basePrice;
    public double BasePrice
    {
        get => _basePrice;
        set => SetProperty(ref _basePrice, value);
    }

    // Must be implemented by derived classes
    public abstract double CalculateFinalPrice();

    public virtual bool Matches(CustomerRequirements requirements)
    {
        if (requirements == null) return false;

        // Check max price
        if (requirements.MaxPrice > 0 && requirements.MaxPrice < CalculateFinalPrice())
            return false;
        
        // Check body type
        if (requirements.PreferredBodyType != BodyType.Any && requirements.PreferredBodyType != BodyType)
            return false;

        // Check year
        if (requirements.MinYear.HasValue && Year < requirements.MinYear)
            return false;

        // Check brand (case insensitive contains)
        if (!string.IsNullOrEmpty(requirements.PreferredBrand) && !Brand.Contains(requirements.PreferredBrand, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    public virtual double CalculateMatchScore(CustomerRequirements requirements)
    {
        if (!Matches(requirements)) return 0;

        double score = 100;
        
        // Lower price is better for the customer
        if (requirements.MaxPrice > 0)
        {
            score -= (CalculateFinalPrice() / requirements.MaxPrice) * 20;
        }

        // Newer car is better
        if (requirements.MinYear.HasValue)
        {
            score += (Year - requirements.MinYear.Value) * 2;
        }

        return Math.Max(0, score);
    }
}
