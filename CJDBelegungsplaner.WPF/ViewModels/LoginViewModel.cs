using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class LoginViewModel : ViewModelBase
{
    #region Properties/Fields

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private string _userNameErrorMessage;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _passwordErrorMessage;

    [ObservableProperty]
    private bool _isLoginInProgress = false;

    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly IAccountService _accountService;
    private readonly IDialogService _dialogService;
    private readonly IDatabaseInitializerService _databaseInitializerService;
    private readonly IHandleResultService _handleResultService;

    #endregion Properties/Fields

    public LoginViewModel(MainWindowViewModelStore mainViewModelStore, IAccountService accountService, IDialogService dialogService, IDatabaseInitializerService databaseInitializerService, IHandleResultService handleResultService)
    {
        _mainViewModelStore = mainViewModelStore;
        _accountService = accountService;
        _dialogService = dialogService;
        _databaseInitializerService = databaseInitializerService;
        _handleResultService = handleResultService;

        if (_mainViewModelStore.Modal.IsOpen)
        {
            _mainViewModelStore.Modal.Close();
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        if (UserName == "admin:database-migrate")
        {
            Migrate();
            return;
        }

        IsLoginInProgress = true;

        bool success = await Task.Run(() => _accountService.Login(UserName, Password));

        IsLoginInProgress = false;

        if (!success)
        {
            return;
        }

        _mainViewModelStore.InitializiseNavigatonBar();
        _mainViewModelStore.Main.NavigateTo(typeof(OccupancyViewModel));
    }

    private void Migrate()
    {
        if (Password != "Chance2015")
        {
            _dialogService.ShowMessageDialog(
                "Passwort faalsch, alta!!", 
                "So nich", 
                MessageBoxImage.Stop);
            return;
        }

        Result<DataServiceResultKind> result;

        result = _databaseInitializerService.InitializeDatabase();

        bool isFlaiure = _handleResultService.Handle(result);

        if (isFlaiure == false)
        {
            _dialogService.ShowMessageDialog(
                "Migration durchgeführt.",
                "Fertig",
                MessageBoxImage.Information);
        }
    }

    // TODO: delete !!!
    [RelayCommand]
    private async Task EasyLogin()
    {
        IsLoginInProgress = true;

        bool success = await Task.Run(() => _accountService.Login("Admin", "Chance2015"));

        IsLoginInProgress = false;

        if (!success)
        {
            return;
        }

        _mainViewModelStore.InitializiseNavigatonBar();
        _mainViewModelStore.Main.NavigateTo(typeof(OccupancyViewModel));
    }
}