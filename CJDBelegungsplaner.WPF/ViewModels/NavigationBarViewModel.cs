using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class NavigationBarViewModel : ViewModelBase
{
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly IAccountService _accountService;

    [ObservableProperty]
    private bool _isReservationButtonChecked;
    [ObservableProperty]
    private bool _isOccupancyButtonChecked;
    [ObservableProperty]
    private bool _isUserListButtonChecked;
    [ObservableProperty]
    private bool _isLogEntryButtonChecked;
    [ObservableProperty]
    private bool _isClassButtonChecked;
    [ObservableProperty]
    private bool _isGuestListButtonChecked;
    [ObservableProperty]
    private bool _isCompanyListButtonChecked;

    public bool HasPermissionForReservationViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.Standard);
    public bool HasPermissionForClassListViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.Standard);
    public bool HasPermissionForOccupancyViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.ReadBedTable);
    public bool HasPermissionForUserListViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.Admin);
    public bool HasPermissionForGuestListViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.Standard);
    public bool HasPermissionForLogEntryListViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.Standard);
    public bool HasPermissionForCompanyListViewModel => _accountService.HasEqualOrHigherPermissionFor(Role.Standard);

    public string UserName => _accountService.User.Name;
    
    public NavigationBarViewModel(
        MainWindowViewModelStore mainViewModelStore,
        IAccountService accountService)
    {
        _mainViewModelStore = mainViewModelStore;
        _accountService = accountService;

        _mainViewModelStore.Main.CurrentViewModelChanged += OnRadioButtonChanged;
    }

    [RelayCommand]
    public void NavigateTo(Type viewModelType)
    {
        _mainViewModelStore.Main.NavigateTo(viewModelType);
    }

    [RelayCommand]
    public void OpenChangePasswordDialog(object? value)
    {
        _mainViewModelStore.Modal.NavigateTo(typeof(ChangePasswordInputFormViewModel),(viewModel) =>
        {
            var form = viewModel as ChangePasswordInputFormViewModel;
            form.ExecuteClose = () => _mainViewModelStore.Modal.Close();
        });
    }

    [RelayCommand]
    public async Task Logout()
    {
        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));
        await _accountService.Logout();
        _mainViewModelStore.Main.NavigateTo(typeof(LoginViewModel));
    }

    public void OnRadioButtonChanged()
    {
        IViewModel currentViewModel = _mainViewModelStore.Main.CurrentViewModel;

        switch (currentViewModel.GetType().Name)
        {
            case "UserListViewModel":
                IsUserListButtonChecked = true;
                break;
            case "OccupancyViewModel":
                IsOccupancyButtonChecked = true;
                break;
            case "ReservationViewModel":
                IsReservationButtonChecked = true;
                break;
            case "LogEntryListViewModel":
                IsLogEntryButtonChecked = true;
                break;
            case "ClassListViewModel":
                IsClassButtonChecked= true;
                break;
            case "GuestListViewModel":
                IsGuestListButtonChecked = true;
                break;
            case "CompanyListViewModel":
                IsCompanyListButtonChecked = true;
                break;
        }
    }

    public override void Dispose()
    {
        _mainViewModelStore.Main.CurrentViewModelChanged -= OnRadioButtonChanged;
    }
}
