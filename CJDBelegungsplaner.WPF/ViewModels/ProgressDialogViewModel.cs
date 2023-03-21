using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class ProgressDialogViewModel : ViewModelBase
{
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly ProgressDialogViewModelStore _progressDialogViewModelStore;
    private readonly IDataConnectionService _dataConnectionService;

    [ObservableProperty]
    private bool isMessageEnabled = false;

    public ProgressDialogViewModel(
        MainWindowViewModelStore mainViewModelStore,
        ProgressDialogViewModelStore progressDialogViewModelStore,
        IDataConnectionService dataConnectionService)
    {
        _mainViewModelStore = mainViewModelStore;
        _progressDialogViewModelStore = progressDialogViewModelStore;
        _dataConnectionService = dataConnectionService;

        _progressDialogViewModelStore.DatabaseConnectionCheck += OnDatabaseConnectionCheck;
    }

    public void OnDatabaseConnectionCheck()
    {
        if (_mainViewModelStore.Main.CurrentViewModel is null)
        {
            throw new ArgumentNullException(nameof(_mainViewModelStore.Main.CurrentViewModel));
        }

        IsMessageEnabled = true;
        CheckDatabaseConnectionAsync();
    }

    private async Task CheckDatabaseConnectionAsync()
    {
        bool canDatabaseConnect = false;

        do
        {
            canDatabaseConnect = await Task.Run(() => _dataConnectionService.DatabaseCanConnect());
            await Task.Delay(3000);
        } while (!canDatabaseConnect);

        _mainViewModelStore.Modal.Close();

        _mainViewModelStore.Main.NavigateTo(_mainViewModelStore.Main.CurrentViewModel.GetType());
    }

    public override void Dispose()
    {
        _progressDialogViewModelStore.DatabaseConnectionCheck -= OnDatabaseConnectionCheck;
        base.Dispose();
    }
}
