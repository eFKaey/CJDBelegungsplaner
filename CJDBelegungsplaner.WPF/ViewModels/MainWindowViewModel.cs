using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CJDBelegungsplaner.WPF.ViewModels;

/// <summary>
/// 
/// </summary>
[INotifyPropertyChanged]
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly IAccountService _accountService;
    private readonly Func<IServiceScope> _createServiceScope;

    private IServiceScope _serviceScope;

    [ObservableProperty]
    private NavigationBarViewModel _navigationBarViewModel;

    public IViewModel? CurrentMainViewModel => _mainViewModelStore.Main.CurrentViewModel;
    [ObservableProperty]
    private bool isMainOpen = true;

    public IViewModel? CurrentModalViewModel => _mainViewModelStore.Modal.CurrentViewModel;
    public bool IsModalOpen
    {
        get => _mainViewModelStore.Modal.IsOpen;
        set
        {
            _mainViewModelStore.Modal.Close();
        }
    }

    public IViewModel? CurrentSubViewModel => _mainViewModelStore.Sub.CurrentViewModel;
    public bool IsSubOpen
    {
        get => _mainViewModelStore.Sub.IsOpen;
        set
        {
            _mainViewModelStore.Sub.Close();
        }
    }

    public bool IsViewBlocked => _mainViewModelStore.IsViewBlocked;

    [ObservableProperty]
    private bool _showNavigationBar = false;

    public MainWindowViewModel(MainWindowViewModelStore mainViewModelStore, Func<IServiceScope> createServiceScope, IAccountService accountService)
    {
        _mainViewModelStore = mainViewModelStore;
        _accountService = accountService;

        _createServiceScope = createServiceScope;

        _mainViewModelStore.Main.CurrentViewModelChanged += OnCurrentMainViewModelChanged;
        _mainViewModelStore.Modal.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
        _mainViewModelStore.Sub.CurrentViewModelChanged += OnCurrentSublViewModelChanged;

        _mainViewModelStore.NavigatonBarInitializised += OnNavigatonBarInitializised;

        _mainViewModelStore.ViewBlockedChanged += OnViewBlockedChanged;

        OnCurrentMainViewModelChanged();
    }

    private void OnCurrentMainViewModelChanged()
    {
        IsSubOpen = false;
        IsMainOpen = true;
        OnPropertyChanged(nameof(CurrentMainViewModel));
        ShowNavigationBar = _accountService.IsAppLoggedIn();
    }

    private void OnCurrentModalViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentModalViewModel));
        OnPropertyChanged(nameof(IsModalOpen));
    }

    private void OnCurrentSublViewModelChanged()
    {
        if (IsSubOpen)
        {
            IsMainOpen = false;
        }
        OnPropertyChanged(nameof(CurrentSubViewModel));
        OnPropertyChanged(nameof(IsSubOpen));
    }

    private void OnNavigatonBarInitializised()
    {
        _serviceScope?.Dispose();
        _serviceScope = _createServiceScope();
        NavigationBarViewModel = _serviceScope.ServiceProvider.GetRequiredService<NavigationBarViewModel>();
    }

    private void OnViewBlockedChanged()
    {
        OnPropertyChanged(nameof(IsViewBlocked));
    }

    [RelayCommand]
    private void CloseSub()
    {
        IsSubOpen = false;
        IsMainOpen = true;
    }
}
