using System;
using System.ComponentModel.DataAnnotations;
using AutoTrade.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoTrade.Models;

public class Customer : ObservableValidator, IStorable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _fullName = string.Empty;
    [Required(ErrorMessage = "ПІБ є обов'язковим")]
    public string FullName
    {
        get => _fullName;
        set 
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                OnPropertyChanged(nameof(FullName));
                return;
            }
            SetProperty(ref _fullName, value, true);
        }
    }

    private string _phone = string.Empty;
    [Required(ErrorMessage = "Телефон є обов'язковим")]
    public string Phone
    {
        get => _phone;
        set 
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                OnPropertyChanged(nameof(Phone));
                return;
            }
            SetProperty(ref _phone, value, true);
        }
    }

    private string _email = string.Empty;
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value ?? string.Empty, true);
    }

    private CustomerRequirements _requirements = new();
    public CustomerRequirements Requirements
    {
        get => _requirements;
        set => SetProperty(ref _requirements, value);
    }
}

public class CustomerRequirements : ObservableValidator
{
    private double _maxPrice;
    [Range(0, 100000000, ErrorMessage = "Бюджет не може бути від'ємним")]
    public double MaxPrice
    {
        get => _maxPrice;
        set => SetProperty(ref _maxPrice, value, true);
    }

    private BodyType _preferredBodyType = BodyType.Any;
    public BodyType PreferredBodyType
    {
        get => _preferredBodyType;
        set => SetProperty(ref _preferredBodyType, value, true);
    }

    private int? _minYear;
    [Range(1900, 2100, ErrorMessage = "Рік має бути в межах 1900-2100")]
    public int? MinYear
    {
        get => _minYear;
        set => SetProperty(ref _minYear, value, true);
    }

    private string _preferredBrand = string.Empty;
    public string PreferredBrand
    {
        get => _preferredBrand;
        set => SetProperty(ref _preferredBrand, value ?? string.Empty, true);
    }
}
