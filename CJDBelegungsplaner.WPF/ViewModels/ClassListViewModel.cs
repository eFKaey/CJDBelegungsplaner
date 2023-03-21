using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CJDBelegungsplaner.WPF.Stores;
using System.Windows.Data;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CJDBelegungsplaner.WPF.ViewModels.Filter;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class ClassListViewModel : ViewModelBase
{
    #region Properties/Fields

    private readonly GuestListViewModelStore _guestListViewModelStore;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly ClassListViewModelStore _classListViewModelStore;
    private readonly DataStore _dataStore;

    private IServiceScope _serviceScope;

    [ObservableProperty]
    private ObservableCollection<Class>? _classes;

    [ObservableProperty]
    private CollectionViewSource _classList = new CollectionViewSource();

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    [ObservableProperty]
    private ClassListFilter? _filter;

    private Class? _deleteClass = null;

    [ObservableProperty]
    private Color _color2 = Color.FromRgb(23,123,234);

    #endregion

    #region Konstruktor

    public ClassListViewModel(
        GuestListViewModelStore guestListViewModelStore,
        MainWindowViewModelStore mainViewModelStore,
        Func<IServiceScope> createServiceScope,
        ClassListViewModelStore classListViewModelStore,
        DataStore dataStore)
    {
        _guestListViewModelStore = guestListViewModelStore;
        _mainViewModelStore = mainViewModelStore;
        _createServiceScope = createServiceScope;
        _classListViewModelStore = classListViewModelStore;
        _dataStore = dataStore;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        Classes = await _dataStore.GetClassesAsync(true);

        if (Classes is null)
        {
            return;
        }

        ClassList.Source = Classes;

        Filter = new ClassListFilter(
            _classListViewModelStore,
            ClassList.View.Refresh);

        ClassList.Filter += Filter.OnFilter;

        _mainViewModelStore.Modal.Close();
    }

    [RelayCommand]
    private void ShowClassForm(Class? editClass)
    {
        _serviceScope = _createServiceScope();
        var form = (ClassInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassInputFormViewModel));
        form.EditEntity = editClass;

        if (form.IsNewEntity)
        {
            form.SaveCompleted = (@class) => Classes.Add(@class);
        }
        else
        {
            form.SaveCompleted = (@class) => ClassList.View.Refresh();
        }

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowDeleteMessage(Class? deleteClass)
    {
        if (deleteClass is null)
        {
            throw new ArgumentNullException(nameof(deleteClass));
        }

        _serviceScope = _createServiceScope();
        var form = (ClassDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassDeleteFormViewModel));
        form.DeleteMessage = "Möchten sie den Gast und alle seine Daten wirklich löschen?";
        form.DeleteEntity = deleteClass;
        form.DeleteCompleted = () => Classes!.Remove(deleteClass);
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
    private void ShowGuests(Class @class)
    {
        _guestListViewModelStore.ClearFilter();
        _guestListViewModelStore.FilterClassName = @class.Name;
        _mainViewModelStore.Main.NavigateTo(typeof(GuestListViewModel));
    }

    public override void Dispose()
    {
        if (Filter is not null)
        {
            Filter.Refresh -= ClassList.View.Refresh;
            Filter.Dispose();
        }
        base.Dispose();
    }

}
