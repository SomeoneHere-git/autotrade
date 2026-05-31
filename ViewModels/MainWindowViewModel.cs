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
    public ObservableCollection<string> AvailableBrands { get; } = new();
    public ObservableCollection<string> AvailableModels { get; } = new();

    public BodyType[] BodyTypes { get; } = (BodyType[])Enum.GetValues(typeof(BodyType));
    public VehicleOrigin[] VehicleOrigins { get; } = (VehicleOrigin[])Enum.GetValues(typeof(VehicleOrigin));
    public SortOption[] SortOptions { get; } = (SortOption[])Enum.GetValues(typeof(SortOption));

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

    private string _searchBrand = string.Empty;
    public string SearchBrand
    {
        get => _searchBrand;
        set => SetProperty(ref _searchBrand, value);
    }

    private string _searchModel = string.Empty;
    public string SearchModel
    {
        get => _searchModel;
        set => SetProperty(ref _searchModel, value);
    }

    private int? _searchMinYear;
    public int? SearchMinYear
    {
        get => _searchMinYear;
        set => SetProperty(ref _searchMinYear, value);
    }

    private BodyType _searchBodyType = BodyType.Any;
    public BodyType SearchBodyType
    {
        get => _searchBodyType;
        set => SetProperty(ref _searchBodyType, value);
    }

    private SortOption _selectedSortOption = SortOption.FinalPrice;
    public SortOption SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (SetProperty(ref _selectedSortOption, value))
            {
                ApplyFilter();
            }
        }
    }

    private bool _sortDescending;
    public bool SortDescending
    {
        get => _sortDescending;
        set
        {
            if (SetProperty(ref _sortDescending, value))
            {
                ApplyFilter();
            }
        }
    }

    public MainWindowViewModel()
    {
        // Commands
        LoadDataCommand = new RelayCommand(LoadData);
        SaveDataCommand = new RelayCommand(SaveData);
        AddVehicleCommand = new RelayCommand(AddVehicle);
        RemoveVehicleCommand = new RelayCommand<Vehicle>(RemoveVehicle);
        AddCustomerCommand = new RelayCommand(AddCustomer);
        RemoveCustomerCommand = new RelayCommand<Customer>(RemoveCustomer);
        MatchVehiclesCommand = new RelayCommand(MatchVehicles);
        ApplyFilterCommand = new RelayCommand(ApplyFilter);
        SetSortOptionCommand = new RelayCommand<SortOption>(SetSortOption);
    }

    public IRelayCommand LoadDataCommand { get; }
    public IRelayCommand SaveDataCommand { get; }
    public IRelayCommand AddVehicleCommand { get; }
    public IRelayCommand<Vehicle> RemoveVehicleCommand { get; }
    public IRelayCommand AddCustomerCommand { get; }
    public IRelayCommand<Customer> RemoveCustomerCommand { get; }
    public IRelayCommand MatchVehiclesCommand { get; }
    public IRelayCommand ApplyFilterCommand { get; }
    public IRelayCommand<SortOption> SetSortOptionCommand { get; }

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
            UpdateAvailableBrands();
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

    public void AddVehicle()
    {
        var vehicle = new Vehicle { Brand = "Нова марка", Model = "Нова модель", BasePrice = 10000.0, Origin = VehicleOrigin.Domestic };
        Vehicles.Add(vehicle);
        SelectedVehicle = vehicle;
        ApplyFilter();
        MatchVehicles();
        UpdateAvailableBrands();
    }

    public void SetSortOption(SortOption option)
    {
        SelectedSortOption = option;
    }

    public void RemoveVehicle(Vehicle? vehicle)
    {
        if (vehicle != null)
        {
            Vehicles.Remove(vehicle);
            ApplyFilter();
            MatchVehicles();
            UpdateAvailableBrands();
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

        if (!string.IsNullOrWhiteSpace(SearchModel))
        {
            query = query.Where(v => v.Model != null && v.Model.Contains(SearchModel, StringComparison.OrdinalIgnoreCase));
        }

        if (SearchMinYear.HasValue)
        {
            query = query.Where(v => v.Year >= SearchMinYear.Value);
        }

        if (SearchBodyType != BodyType.Any)
        {
            query = query.Where(v => v.BodyType == SearchBodyType);
        }

        query = SelectedSortOption switch
        {
            SortOption.Brand => SortDescending ? query.OrderByDescending(v => v.Brand) : query.OrderBy(v => v.Brand),
            SortOption.Model => SortDescending ? query.OrderByDescending(v => v.Model) : query.OrderBy(v => v.Model),
            SortOption.Year => SortDescending ? query.OrderByDescending(v => v.Year) : query.OrderBy(v => v.Year),
            SortOption.BasePrice => SortDescending ? query.OrderByDescending(v => v.BasePrice) : query.OrderBy(v => v.BasePrice),
            SortOption.FinalPrice => SortDescending ? query.OrderByDescending(v => v.CalculateFinalPrice()) : query.OrderBy(v => v.CalculateFinalPrice()),
            SortOption.Origin => SortDescending ? query.OrderByDescending(v => v.Origin) : query.OrderBy(v => v.Origin),
            _ => query.OrderBy(v => v.CalculateFinalPrice())
        };

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

    public void UpdateAvailableBrands()
    {
        var brands = Vehicles.Select(v => v.Brand)
            .Where(b => !string.IsNullOrWhiteSpace(b))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(b => b)
            .ToList();
            
        AvailableBrands.Clear();
        foreach (var b in brands)
        {
            AvailableBrands.Add(b);
        }

        var models = Vehicles.Select(v => v.Model)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(m => m)
            .ToList();
            
        AvailableModels.Clear();
        foreach (var m in models)
        {
            AvailableModels.Add(m);
        }
    }
}
