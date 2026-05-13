using System;
using AutoTrade.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoTrade.Models;

public class Customer : ObservableObject, IStorable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private CustomerRequirements _requirements = new();
    public CustomerRequirements Requirements
    {
        get => _requirements;
        set => SetProperty(ref _requirements, value);
    }
}

public class CustomerRequirements : ObservableObject
{
    private double _maxPrice;
    public double MaxPrice
    {
        get => _maxPrice;
        set => SetProperty(ref _maxPrice, value);
    }

    private BodyType _preferredBodyType = BodyType.Any;
    public BodyType PreferredBodyType
    {
        get => _preferredBodyType;
        set => SetProperty(ref _preferredBodyType, value);
    }

    private int? _minYear;
    public int? MinYear
    {
        get => _minYear;
        set => SetProperty(ref _minYear, value);
    }

    private string _preferredBrand = string.Empty;
    public string PreferredBrand
    {
        get => _preferredBrand;
        set => SetProperty(ref _preferredBrand, value);
    }
}
