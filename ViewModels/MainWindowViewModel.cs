using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AutoTrade.Models;
using AutoTrade.Services;
using CommunityToolkit.Mvvm.Input;

namespace AutoTrade.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly DataService _dataService = new();
    private readonly MatchingEngine _matchingEngine = new();
    private readonly string _dataPath = "autotrade_data.json";

    public ObservableCollection<Vehicle> Vehicles { get; } = new();
    public ObservableCollection<Vehicle> FilteredVehicles { get; } = new();
    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Vehicle> MatchedVehicles { get; } = new();

    public BodyType[] BodyTypes { get; } = (BodyType[])Enum.GetValues(typeof(BodyType));

    private Vehicle? _selectedVehicle;
    public Vehicle? SelectedVehicle
    {
        get => _selectedVehicle;
        set => SetProperty(ref _selectedVehicle, value);
    }

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            var old = _selectedCustomer;
            if (SetProperty(ref _selectedCustomer, value))
            {
                OnSelectedCustomerChanged(old, value);
            }
        }
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    // Filter properties (Scenario 4)
    private string _searchBrand = string.Empty;
    public string SearchBrand
    {
        get => _searchBrand;
        set => SetProperty(ref _searchBrand, value);
    }

    private int? _searchMinYear;
    public int? SearchMinYear
    {
        get => _searchMinYear;
        set => SetProperty(ref _searchMinYear, value);
    }



    public MainWindowViewModel()
    {
        // Commands
        LoadDataCommand = new RelayCommand(LoadData);
        SaveDataCommand = new RelayCommand(SaveData);
        AddDomesticVehicleCommand = new RelayCommand(AddDomesticVehicle);
        AddForeignVehicleCommand = new RelayCommand(AddForeignVehicle);
        RemoveVehicleCommand = new RelayCommand<Vehicle>(RemoveVehicle);
        AddCustomerCommand = new RelayCommand(AddCustomer);
        RemoveCustomerCommand = new RelayCommand<Customer>(RemoveCustomer);
        MatchVehiclesCommand = new RelayCommand(MatchVehicles);
        ApplyFilterCommand = new RelayCommand(ApplyFilter);
    }

    public IRelayCommand LoadDataCommand { get; }
    public IRelayCommand SaveDataCommand { get; }
    public IRelayCommand AddDomesticVehicleCommand { get; }
    public IRelayCommand AddForeignVehicleCommand { get; }
    public IRelayCommand<Vehicle> RemoveVehicleCommand { get; }
    public IRelayCommand AddCustomerCommand { get; }
    public IRelayCommand<Customer> RemoveCustomerCommand { get; }
    public IRelayCommand MatchVehiclesCommand { get; }
    public IRelayCommand ApplyFilterCommand { get; }

    // React to SelectedCustomer changes
    private void OnSelectedCustomerChanged(Customer? oldValue, Customer? newValue)
    {
        if (oldValue?.Requirements != null)
        {
            oldValue.Requirements.PropertyChanged -= Requirements_PropertyChanged;
        }

        if (newValue?.Requirements != null)
        {
            newValue.Requirements.PropertyChanged += Requirements_PropertyChanged;
        }

        MatchVehicles();
    }

    private void Requirements_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        MatchVehicles();
    }

    public void LoadData()
    {
        try
        {
            var data = _dataService.LoadData(_dataPath);
            
            Vehicles.Clear();
            foreach (var v in data.Vehicles) Vehicles.Add(v);

            Customers.Clear();
            foreach (var c in data.Customers) Customers.Add(c);

            ApplyFilter(); // Refresh filtered view
            StatusMessage = "Data loaded successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
    }

    public void SaveData()
    {
        try
        {
            var data = new AppData
            {
                Vehicles = Vehicles.ToList(),
                Customers = Customers.ToList()
            };
            
            _dataService.SaveData(_dataPath, data);
            StatusMessage = "Data saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving data: {ex.Message}";
        }
    }

    public void AddDomesticVehicle()
    {
        var vehicle = new DomesticVehicle { Brand = "New Brand", Model = "New Model", BasePrice = 10000.0 };
        Vehicles.Add(vehicle);
        SelectedVehicle = vehicle;
        ApplyFilter();
        MatchVehicles();
    }

    public void AddForeignVehicle()
    {
        var vehicle = new ForeignVehicle { Brand = "New Brand", Model = "New Model", BasePrice = 10000.0 };
        Vehicles.Add(vehicle);
        SelectedVehicle = vehicle;
        ApplyFilter();
        MatchVehicles();
    }

    public void RemoveVehicle(Vehicle? vehicle)
    {
        if (vehicle != null)
        {
            Vehicles.Remove(vehicle);
            ApplyFilter();
            MatchVehicles();
        }
    }

    public void AddCustomer()
    {
        var customer = new Customer { FullName = "New Customer" };
        Customers.Add(customer);
        SelectedCustomer = customer;
    }

    public void RemoveCustomer(Customer? customer)
    {
        if (customer != null)
        {
            Customers.Remove(customer);
            if (SelectedCustomer == customer)
            {
                SelectedCustomer = null;
            }
        }
    }

    public void MatchVehicles()
    {
        MatchedVehicles.Clear();

        if (SelectedCustomer == null)
        {
            StatusMessage = "Please select a customer first.";
            return;
        }

        var matches = _matchingEngine.FindMatches(Vehicles, SelectedCustomer.Requirements);
        
        foreach (var m in matches)
        {
            MatchedVehicles.Add(m);
        }

        if (!MatchedVehicles.Any())
        {
            StatusMessage = "Немає авто, що відповідають вимогам"; // Scenario 9 alternative
        }
        else
        {
            StatusMessage = $"Found {MatchedVehicles.Count} matching vehicles.";
        }
    }

    public void ApplyFilter()
    {
        var query = Vehicles.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchBrand))
        {
            query = query.Where(v => v.Brand != null && v.Brand.Contains(SearchBrand, StringComparison.OrdinalIgnoreCase));
        }

        if (SearchMinYear.HasValue)
        {
            query = query.Where(v => v.Year >= SearchMinYear.Value);
        }

        // Scenario 3: LINQ sorting implementation
        query = query.OrderBy(v => v.CalculateFinalPrice());

        var result = query.ToList();
        FilteredVehicles.Clear();
        foreach (var v in result)
        {
            FilteredVehicles.Add(v);
        }

        if (FilteredVehicles.Count == 0 && Vehicles.Count > 0)
        {
            StatusMessage = "За вашим запитом авто не знайдено"; // Scenario 4 alternative
        }
        else
        {
            StatusMessage = $"Filtered to {FilteredVehicles.Count} vehicles.";
        }
    }


}
