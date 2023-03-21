using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    #endregion Properties/Fields

    public LoginViewModel(MainWindowViewModelStore mainViewModelStore, IAccountService accountService)
    {
        _mainViewModelStore = mainViewModelStore;
        _accountService = accountService;

        if (_mainViewModelStore.Modal.IsOpen) 
        {
            _mainViewModelStore.Modal.Close();
        }
    }

    [RelayCommand]
    private async Task Login()
    {
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