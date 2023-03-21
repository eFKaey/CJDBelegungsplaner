using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Forms;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using CJDBelegungsplaner.WPF.Views.Forms;
using CJDBelegungsplaner.WPF.Views.InputForms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;
using static CJDBelegungsplaner.WPF.ViewModels.Filter.CompanyListViewModel;

namespace CJDBelegungsplaner.WPF.ViewModels
{
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
        private object? _dialogContent;

        [ObservableProperty]
        private bool _isDialogOpen;

        [ObservableProperty]
        private DeleteDialogViewModel? _deleteForm;


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
            IDataHelperSevice dataHelperSevice)
        {
            _companyListViewModelStore = companyListViewModelStore;
            _accountService = accountService;
            _companyDataService = companyDataService;
            _handleResultService= handleResultService;
            _createServiceScope = createServiceScope;
            _mainViewModelStore = mainViewModelStore;
            _dataHelperSevice = dataHelperSevice;

            _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

            LoadDataAsync();
        }

        #endregion

        private async Task LoadDataAsync()
        {
            if (_companyListViewModelStore.IsCompanyListParameter)
            {
                Companies = _companyListViewModelStore.CompaniesListParameter!;
            }
            else
            {
                Companies = await _dataHelperSevice.GetCollectionAsync(_companyDataService.GetAllAsync);
            }

            if(Companies is null)
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
            _editCompany= editCompany;

            _serviceScope = _createServiceScope();
            Form = (CompanyInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(CompanyInputFormViewModel));
            Form.Name = _name;
            Form.PhoneNumber = _phoneNumber;
            Form.Email = _email;
            Form.City = _city;
            Form.Street = _street;
            Form.PostCode = _postCode;
            Form.HouseNumber = _houseNumber;
            Form.Description = _description;
            Form.SetProperties(editCompany);
            DialogContent = new CompanyInputFormView() { DataContext = this };

            if (IsCompanyNew
            && !string.IsNullOrEmpty(Filter?.Name)
            && _companyNames.Contains(Filter.Name!))
            {
                Form.Name = Filter.Name;
            }

            OpenDialog();
        }

        [RelayCommand]
        private void ShowDeleteMessage(Company? deleteCompany)
        {
            if (deleteCompany is null)
            {
                throw new ArgumentNullException(nameof(deleteCompany));
            }

            _serviceScope?.Dispose();
            _serviceScope = _createServiceScope();
            DeleteForm = (DeleteDialogViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(DeleteDialogViewModel));
            DeleteForm.DeleteMessage = "Möchten sie den Gast und alle seine Daten wirklich löschen?";
            DialogContent = new DeleteDialogView() { DataContext = this };

            _deleteCompany = deleteCompany;

            OpenDialog();
        }

        private ObservableCollection<Company> GetCompanies()
        {
            Result<CompanyDataServiceResultKind, ICollection<Company>> result = _companyDataService.GetAllAsync().Result;
            bool isFailure = _handleResultService.Handle(result);

            if (isFailure)
            {
                return null;
            }
            else
            {
                return new ObservableCollection<Company>(result.Content!);
            }
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

        [RelayCommand]
        private async Task SaveCompany()
        {
            if (!IsDialogOpen)
            {
                return;
            }
            if (Form is null)
            {
                throw new NullReferenceException(nameof(Form));
            }
            if (Companies is null)
            {
                throw new NullReferenceException(nameof(Companies));
            }

            Form.ValidateProperty(Form.Name, nameof(Form.Name));
            if (Form.HasErrors)
            {
                return;
            }


            bool isFailure;

            Form.IsFormEnabled = false;

            if (IsCompanyNew)
            {
                isFailure = await CreateCompany();
            }
            else
            {
                isFailure = await UpdateCompany();
            }

            Form.IsFormEnabled = true;

            if (isFailure)
            {
                return;
            }

            _editCompany = null;

            CloseDialog();
        }

        private async Task<bool> CreateCompany()
        {
            bool isSuccess = false;

            Result<CompanyDataServiceResultKind, Company> result;

            Company newCompany = new Company
            {
                Name = Form.Name,
                Description = Form.Description,
                PhoneNumber = Form.PhoneNumber,
                Email = Form.Email,
                Address = new Address
                {
                    Street = Form.Street,
                    HouseNumber = Form.HouseNumber,
                    City = Form.City,
                    PostCode = Form.PostCode
                },
                Created = DateTime.Now
            };
            result = await Task.Run( () => _companyDataService.CreateAsync(newCompany));

            bool isFailure = _handleResultService.Handle(result);

            if (isFailure)
            {
                return isFailure;
            }

            Company company = result.Content;

            Companies.Add(company);

            _accountService.MakeUserLogEntry($"Firma '{Form.Name}' angelegt.");

            return isSuccess;
        }

        private async Task<bool> UpdateCompany()
        {
            bool isSuccess = false;

            Result<CompanyDataServiceResultKind> result;
            Company tempCompany = _editCompany!.Clone();

            _editCompany.Name = Form!.Name;
            _editCompany.Description = Form.Description;
            _editCompany.Email = Form.Email;
            _editCompany.PhoneNumber = Form.PhoneNumber;
            _editCompany.Address.Street = Form.Street;
            _editCompany.Address.HouseNumber = Form.HouseNumber;
            _editCompany.Address.City = Form.City;
            _editCompany.Address.PostCode = Form.PostCode;

            result = await Task.Run(() => _companyDataService.UpdateAsync(_editCompany!.Id, _editCompany));

            bool isFailure = _handleResultService.Handle(result);

            if(isFailure)
            {
                tempCompany.CopyValues(_editCompany);
                return isFailure;
            }

            CompanyList.View.Refresh();

            _accountService.MakeUserLogEntry($"Firma '{Form.Name}' bearbeitet.");

            return isSuccess;

        }

        [RelayCommand]
        private async Task Delete()
        {
            if (!IsDialogOpen)
            {
                return;
            }
            if (_deleteCompany is null)
            {
                throw new ArgumentNullException(nameof(_deleteCompany));
            }

            Result<CompanyDataServiceResultKind> result = await Task.Run(() => _companyDataService.DeleteAsync(_deleteCompany.Id));

            bool isFailure = _handleResultService.Handle(result);

            if (isFailure)
            {
                return;
            }

            Companies!.Remove(_deleteCompany);

            _accountService.MakeUserLogEntry($"Firma '{_deleteCompany.Name}' gelöscht.");

            _deleteCompany = null;

            CloseDialog();

        }

        public override void Dispose()
        {
            Filter.Refresh -= CompanyList.View.Refresh;
            Filter.Dispose();
            base.Dispose();
        }
    }
}
