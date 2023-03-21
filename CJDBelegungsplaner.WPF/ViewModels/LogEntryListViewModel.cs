using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Filter;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class LogEntryListViewModel : ViewModelBase
{
    #region Properties/Fields

    private readonly LogEntryListViewModelStore _logEntryListViewModelStore;
    private readonly ILogEntryDataService _logEntryDataService;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly IDataHelperSevice _dataHelperSevice;
    private readonly DataStore _dataStore;

    [ObservableProperty]
    private ObservableCollection<LogEntry>? _logEntries;

    [ObservableProperty]
    private CollectionViewSource _logEntryList = new CollectionViewSource();

    [ObservableProperty]
    private ObservableCollection<User>? _users;

    [ObservableProperty]
    private LogEntryListFilter? _filter;

    public List<string> UserNames { get; private set; } = new List<string>();

    #endregion

    #region Konstruktor

    public LogEntryListViewModel(
        LogEntryListViewModelStore logEntryListViewModelStore,
        ILogEntryDataService logEntryDataService,
        MainWindowViewModelStore mainViewModelStore,
        IDataHelperSevice dataHelperSevice,
        DataStore dataStore)
    {
        _logEntryListViewModelStore = logEntryListViewModelStore;
        _logEntryDataService = logEntryDataService;
        _mainViewModelStore = mainViewModelStore;
        _dataHelperSevice = dataHelperSevice;
        _dataStore = dataStore;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        Users = await _dataStore.GetUsersAsync();

        if (Users is null)
        {
            return;
        }

        LogEntries = await _dataHelperSevice.GetCollectionAsync(_logEntryDataService.GetAllAsync);

        if (LogEntries is null)
        {
            return;
        }

        UserNames = _dataHelperSevice.GetNames(Users);
        LogEntryList.Source = LogEntries;

        Filter = new LogEntryListFilter(
            _logEntryListViewModelStore,
            LogEntryList.View.Refresh,
            UserNames.ConvertAll(n => n));

        LogEntryList.Filter += Filter.OnFilter;

        _mainViewModelStore.Modal.Close();
    }

    public override void Dispose()
    {
        if (Filter is not null)
        {
            Filter.Refresh -= LogEntryList.View.Refresh;
            Filter.Dispose();
        }
        base.Dispose();
    }
}
