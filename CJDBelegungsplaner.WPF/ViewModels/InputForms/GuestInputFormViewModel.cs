using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Utility.ValidationAttributes;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using System;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

/// <summary>
/// Beinhaltet die Attribute, Felder und Validation-Logik für die Eingabeform der View.
/// </summary>
public partial class GuestInputFormViewModel : InputFormBase<Guest>
{
    #region Input Fields/Properies

    /// Wichtige Hinweise für die Properties:
    ///     - Alle Bindings für jegliche Art der Eingabefelder müssen vom Type string? (nullable) sein.
    ///       (Z.B. Datumsfelder müssen vorallem vom Typ string sein, damit auch eine leeres Feld möglich ist. (Mit dem Typ DateTime würde immer etwas drin stehen.))
    ///     - Datumsfelder müssen mit string.Empty initialisiert sein, sonst spinnt die Validation.
    ///     - Comboboxen dürfen keine Initialisation haben.

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(2, ErrorMessage = "Muss länger sein!")]
    private string? _firstName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(2, ErrorMessage = "Muss länger sein!")]
    private string? _lastName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [IsDateTime(ErrorMessage = "Muss ein valides Datum sein!")]
    //[YoungerThan("1.1.1900", ErrorMessage = "Datum ist zu klein!")]
    [IsNotFuture(ErrorMessage = "Zukunft ist noch nicht eingebaut!")]
    private string? _birthdate = string.Empty;

    [ObservableProperty]
    //[NotifyDataErrorInfo]
    //[IsElementOf(nameof(ClassNames), ErrorMessage = "Muss existierende Klasse sein!")]
    private Class? _class;

    [ObservableProperty]
    //[NotifyDataErrorInfo]
    //[IsElementOf(nameof(CompanyNames), ErrorMessage = "Muss existierende Firma sein!")]
    private Company? _company;

    [ObservableProperty]
    private string? _information;

    [ObservableProperty]
    private string? _phoneNumber = string.Empty;

    [ObservableProperty]
    private string? _email = string.Empty;

    [ObservableProperty]
    private string? _street = string.Empty;

    [ObservableProperty]
    private string? _houseNumber = string.Empty;

    [ObservableProperty]
    private string? _postCode = string.Empty;

    [ObservableProperty]
    private string? _city = string.Empty;

    #endregion

    #region Data Fields/Properies

    [ObservableProperty]
    private ObservableCollection<Class>? _classes;
    [ObservableProperty]
    private ObservableCollection<Company>? _companies;

    public override Guest? EditEntity
    {
        get { return base.EditEntity; }
        set
        {
            base.EditEntity = value;

            if (EditEntity is null)
            {
                return;
            }

            FirstName = EditEntity.FirstName;
            LastName = EditEntity.LastName;
            Birthdate = EditEntity.Birthdate.ToString("d");
            Class = EditEntity.Class;
            Company = EditEntity.Company;
            Information = EditEntity.Information;
            PhoneNumber = EditEntity.PhoneNumber;
            Email = EditEntity.Email;
            Street = EditEntity.Address.Street;
            HouseNumber = EditEntity.Address.HouseNumber;
            PostCode = EditEntity.Address.PostCode;
            City = EditEntity.Address.City;
        }
    }

    #endregion

    #region Dialog Fields/Properties

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private CompanyInputFormViewModel _companyForm;

    [ObservableProperty]
    private ClassInputFormViewModel _classForm;

    [ObservableProperty]
    private bool _isDialogOpen;

    #endregion

    #region Injection Fields and Constructor

    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly IGuestDataService _guestDataService;
    private readonly DataStore _dataStore;
    private readonly Func<IServiceScope> _createServiceScope;

    private IServiceScope _serviceScope;

    public GuestInputFormViewModel(
        IHandleResultService handleResultService,
        IAccountService accountService,
        IGuestDataService guestDataService,
        DataStore dataStore,
        Func<IServiceScope> createServiceScope)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _guestDataService = guestDataService;
        _dataStore = dataStore;
        _createServiceScope = createServiceScope;

        LoadAsync();
    }

    #endregion

    private async Task LoadAsync()
    {
        Classes = await _dataStore.GetClassesAsync();
        Companies = await _dataStore.GetCompaniesAsync();
    }

    public override async Task<(bool, Guest)> CreateAsync()
    {
        bool isSuccess = false;

        Result<DataServiceResultKind, Guest> result;

        Guest newGuest = new Guest
        {
            FirstName = FirstName,
            LastName = LastName,
            Birthdate = DateTime.Parse(Birthdate),
            Class = Class,
            Company = Company,
            Information = Information,
            PhoneNumber = PhoneNumber,
            Email = Email,
            Address = new Address
            {
                Street = Street,
                HouseNumber = HouseNumber,
                PostCode = PostCode,
                City = City
            },
            Created = DateTime.Now
        };

        result = await Task.Run(() => _guestDataService.CreateAsync(newGuest));

        bool isFailure = _handleResultService.Handle(result);

        Guest guest = result.Content!;

        if (isFailure)
        {
            return (isFailure, guest);
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) '{FirstName} {LastName}' angelegt.");

        return (isSuccess, guest);
    }

    public override async Task<(bool, Guest)> UpdateAsync()
    {
        bool isSuccess = false;

        Result<DataServiceResultKind, Guest> result;

        Guest tempGuest = EditEntity!.Clone();

        EditEntity.FirstName = FirstName;
        EditEntity.LastName = LastName;
        EditEntity.Birthdate = DateTime.Parse(Birthdate);
        EditEntity.Class = Class;
        EditEntity.Company = Company;
        EditEntity.Information = Information;
        EditEntity.PhoneNumber = PhoneNumber;
        EditEntity.Email = Email;
        EditEntity.Address.Street = Street;
        EditEntity.Address.HouseNumber = HouseNumber;
        EditEntity.Address.PostCode = PostCode;
        EditEntity.Address.City = City;

        result = await Task.Run(() => _guestDataService.UpdateAsync(EditEntity.Id, EditEntity));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempGuest.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) '{FirstName} {LastName}' bearbeitet.");

        return (isSuccess, EditEntity);
    }

    [RelayCommand]
    private void ShowCompanyForm()
    {
        _serviceScope = _createServiceScope();
        CompanyForm = (CompanyInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(CompanyInputFormViewModel));
        CurrentFormViewModel = CompanyForm;

        OpenDialog();

        System.Windows.MessageBox.Show("unfertig");
    }

    [RelayCommand]
    private void ShowClassForm()
    {
        if (Classes is null)
        {
            throw new NullReferenceException(nameof(Classes));
        }

        _serviceScope = _createServiceScope();
        ClassForm = (ClassInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassInputFormViewModel));
        ClassForm.SaveCompleted = (@class) =>
        {
            Classes.Add(@class);
            Class = @class;
        };
        ClassForm.ExecuteClose = CloseDialog;
        CurrentFormViewModel = ClassForm;

        OpenDialog();
    }

    private void OpenDialog()
    {
        IsDialogOpen = true;
    }

    [RelayCommand]
    private void CloseDialog()
    {
        IsDialogOpen = false;
        CurrentFormViewModel = null;
        _serviceScope?.Dispose();
    }
}
