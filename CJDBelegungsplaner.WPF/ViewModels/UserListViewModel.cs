using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class UserListViewModel : ViewModelBase
{
    #region Properties/Fields

    private readonly IDialogService _dialogService;
    private readonly IAccountService _accountService;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly LogEntryListViewModelStore _logEntryListViewModelStore;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly DataStore _dataStore;

    private IServiceScope _serviceScope;

    [ObservableProperty]
    private ObservableCollection<User>? _users;

    [ObservableProperty]
    private CollectionViewSource _userList = new CollectionViewSource();

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    private User? _deleteUser = null;

    #endregion

    #region Konstruktor

    public UserListViewModel(
        IDialogService dialogService,
        IAccountService accountService,
        MainWindowViewModelStore mainViewModelStore,
        LogEntryListViewModelStore logEntryListViewModelStore,
        Func<IServiceScope> createServiceScope,
        DataStore dataStore)
    {
        _dialogService = dialogService;
        _accountService = accountService;
        _mainViewModelStore = mainViewModelStore;
        _logEntryListViewModelStore = logEntryListViewModelStore;
        _createServiceScope = createServiceScope;
        _dataStore = dataStore;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        Users = await _dataStore.GetUsersAsync(true);

        if (Users is null)
        {
            return;
        }

        UserList.Source = Users;

        _mainViewModelStore.Modal.Close();
    }

    [RelayCommand]
    private void ShowUserForm(User editUser)
    {
        _serviceScope = _createServiceScope();
        var form = (UserInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(UserInputFormViewModel));
        form.EditEntity = editUser;

        if (form.IsNewEntity)
        {
            form.SaveCompleted = (user, formWhileSaving) => Users.Add(user);
        }
        else
        {
            if (!_accountService.HasHigherPermissionFor(editUser.Role))
            {
                _dialogService.ShowMessageDialog("Keine Rechte.", "So nich...", MessageBoxImage.Warning);
                _serviceScope?.Dispose();
                return;
            }

            form.IsPasswordEnabled = false;
            form.ShowCheckBoxForPasswordTextBoxes = true;
            form.SaveCompleted = (user, formWhileSaving) => UserList.View.Refresh();
        }

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowDeleteMessage(User? deleteUser)
    {
        if (deleteUser is null)
        {
            throw new ArgumentNullException(nameof(deleteUser));
        }

        if ( ! _accountService.HasHigherPermissionFor(deleteUser.Role))
        {
            _dialogService.ShowMessageDialog("Keine Rechte.", "So nich...", MessageBoxImage.Warning);
            return;
        }

        if (_accountService.User == deleteUser)
        {
            _dialogService.ShowMessageDialog("Du kannst dich nicht selber löschen!", "So nich...", MessageBoxImage.Warning);
            return;
        }

        _serviceScope = _createServiceScope();
        var form = (UserDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(UserDeleteFormViewModel));
        form.DeleteMessage = "Möchten sie den User und alle seine Daten wirklich löschen?";
        form.DeleteEntity = deleteUser;
        form.DeleteCompleted = () => Users!.Remove(deleteUser);
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

    [RelayCommand]
    private void ShowLogEntries(User user)
    {
        _logEntryListViewModelStore.ClearFilter();
        _logEntryListViewModelStore.FilterName = user.Name;
        _mainViewModelStore.Main.NavigateTo(typeof(LogEntryListViewModel));
    }
}