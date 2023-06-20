using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class CompanyInputFormViewModel : InputFormBase<Company>
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [MinLength(2, ErrorMessage = "Muss länger sein!")]
    private string? _name = string.Empty;

    [ObservableProperty]
    private string? _description = string.Empty;

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

    public override Company? EditEntity
    {
        get { return base.EditEntity; }
        set
        {
            base.EditEntity = value;

            if (EditEntity is null)
            {
                return;
            }

            Name = EditEntity.Name;
            Description = EditEntity.Description;
            PhoneNumber = EditEntity.PhoneNumber;
            Email = EditEntity.Email;
            Street = EditEntity.Address.Street;
            HouseNumber = EditEntity.Address.HouseNumber;
            PostCode = EditEntity.Address.PostCode;
            City = EditEntity.Address.City;
        }
    }


    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly ICompanyDataService _companyDataService;
    private readonly DataStore _dataStore;
    private readonly Func<IServiceScope> _createServiceScope;

    private IServiceScope _serviceScope;

    public CompanyInputFormViewModel(IHandleResultService handleResultService,
        IAccountService accountService,
        ICompanyDataService companyDataService,
        DataStore dataStore,
        Func<IServiceScope> createServiceScope)
    {
        _handleResultService = handleResultService;
        _accountService = accountService;
        _companyDataService = companyDataService;
        _dataStore = dataStore;
        _createServiceScope = createServiceScope;
    }

    public override async Task<(bool, Company)> CreateAsync()
    {
        bool isSuccess = false;

        Result<CompanyDataServiceResultKind, Company> result;

        Company newCompany = new Company
        {
            Name = Name,
            Description = Description,
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

        result = await Task.Run(() => _companyDataService.CreateAsync(newCompany));

        bool isFailure = _handleResultService.Handle(result);

        Company company = result.Content!;

        if (isFailure)
        {
            return (isFailure, company);
        }


        _accountService.MakeUserLogEntry($"Firma '{Name}' angelegt.");

        return (isSuccess, company);
    }

    public override async Task<(bool, Company)> UpdateAsync()
    {
        bool isSuccess = false;

        Result<CompanyDataServiceResultKind, Company> result;

        Company tempCompany = EditEntity!.Clone();

        EditEntity.Name = Name;
        EditEntity.Description = Description;
        EditEntity.PhoneNumber = PhoneNumber;
        EditEntity.Email = Email;
        EditEntity.Address.Street = Street;
        EditEntity.Address.HouseNumber = HouseNumber;
        EditEntity.Address.PostCode = PostCode;
        EditEntity.Address.City = City;

        result = await Task.Run(() => _companyDataService.UpdateAsync(EditEntity.Id, EditEntity));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempCompany.CopyValues(EditEntity);

            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Firma '{Name}' bearbeitet.");

        return (isSuccess, EditEntity);
    }

    private void OpenDialog()
    {
        IsDialogOpen = true;
    }

    private void CloseDialog()
    {
        IsDialogOpen = false;
        CurrentFormViewModel = null;
        _serviceScope?.Dispose();
    }
}
