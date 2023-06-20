using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;
using static CJDBelegungsplaner.WPF.ViewModels.Filter.CompanyListViewModel;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class CompanyListViewModel : ViewModelBase
{
    #region Properties/Fields
    private readonly CompanyListViewModelStore _companyListViewModelStore;
    private readonly IAccountService _accountService;
    private readonly ICompanyDataService _companyDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly IDataHelperSevice _dataHelperSevice;
    private readonly DataStore _dataStore;

    private IServiceScope _serviceScope;

    private string _name;
    private string _city;
    private string _postCode;
    private string _street;
    private string _houseNumber;
    private string _email;
    private string _phoneNumber;
    private string _description;

    [ObservableProperty]
    private ObservableCollection<Company> _companies;

    [ObservableProperty]
    private CollectionViewSource _companyList = new CollectionViewSource();

    private List<string> _companyNames = new List<string>();

    [ObservableProperty]
    private CompanyInputFormViewModel? _form;

    [ObservableProperty]
    private CompanyListFilter? _filter;

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private object? _dialogContent;

    [ObservableProperty]
    private bool _isDialogOpen;


    private Company? _editCompany = null;
    private Company? _deleteCompany = null;
    private bool IsCompanyNew => _editCompany is null;
    #endregion

    #region Konstruktor

    public CompanyListViewModel(
        CompanyListViewModelStore companyListViewModelStore,
        IAccountService accountService,
        ICompanyDataService companyDataService,
        IHandleResultService handleResultService,
        Func<IServiceScope> createServiceScope,
        MainWindowViewModelStore mainViewModelStore,
        IDataHelperSevice dataHelperSevice,
        DataStore dataStore)
    {
        _companyListViewModelStore = companyListViewModelStore;
        _accountService = accountService;
        _companyDataService = companyDataService;
        _handleResultService = handleResultService;
        _createServiceScope = createServiceScope;
        _mainViewModelStore = mainViewModelStore;
        _dataHelperSevice = dataHelperSevice;
        _dataStore = dataStore;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        Companies = await _dataStore.GetCompaniesAsync(true);

        if (Companies is null)
        {
            return;
        }

        CompanyList.Source = Companies;

        Filter = new CompanyListFilter(_companyListViewModelStore, CompanyList.View.Refresh);

        CompanyList.Filter += Filter.OnFilter;

        _mainViewModelStore.Modal.Close();
    }

    [RelayCommand]
    private void ShowCompanyForm(Company? editCompany)
    {
        _serviceScope = _createServiceScope();
        var form = (CompanyInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(CompanyInputFormViewModel));
        form.EditEntity = editCompany;

        if (form.IsNewEntity)
        {
            form.SaveCompleted = (company, formWhileSaving) => Companies.Add(company);
        }
        else
        {
            form.SaveCompleted = (guest, formWhileSaving) => CompanyList.View.Refresh();
        }

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }


    [RelayCommand]
    private void ShowDeleteMessage(Company? deleteCompany)
    {
        if (deleteCompany is null)
        {
            throw new ArgumentNullException(nameof(deleteCompany));
        }

        _serviceScope = _createServiceScope();
        var form = (CompanyDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(CompanyDeleteFormViewModel));
        form.DeleteMessage = "Möchten sie die Firma und alle ihre Daten wirklich löschen?";
        form.DeleteEntity = deleteCompany;
        form.DeleteCompleted = () => Companies!.Remove(deleteCompany);
        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    private void OpenDialog()
    {
        IsDialogOpen = true;
        _mainViewModelStore.BlockView(true);
    }

    [RelayCommand]
    private void CloseDialog()
    {
        IsDialogOpen = false;
        _serviceScope?.Dispose();
        _mainViewModelStore.BlockView(false);
    }

    public override void Dispose()
    {
        if (Filter is not null)
        {
            Filter.Refresh -= CompanyList.View.Refresh;
            Filter.Dispose();
        }
        base.Dispose();
    }
}