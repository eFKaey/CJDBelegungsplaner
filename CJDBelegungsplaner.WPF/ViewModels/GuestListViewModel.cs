using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.Input;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System.ComponentModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class GuestListViewModel : ViewModelBase
{
    #region Properties/Fields

    [ObservableProperty]
    private ObservableCollection<Guest>? _guests;

    [ObservableProperty]
    private CollectionViewSource _guestList = new CollectionViewSource();

    [ObservableProperty]
    private ObservableCollection<Class>? _classes;

    private List<string> _classNames = new List<string>();

    [ObservableProperty]
    private ObservableCollection<Company>? _companies;

    private List<string> _companyNames = new List<string>();

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    [ObservableProperty]
    private GuestListFilter? _filter;

    #endregion

    #region Konstruktor

    private readonly GuestListViewModelStore _guestListViewModelStore;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly ClassListViewModelStore _classListViewModelStore;
    private readonly CompanyListViewModelStore _companyListViewModelStore;
    private readonly IDataHelperSevice _dataHelperSevice;
    private readonly GuestDetailsViewModelStore _guestDetailsViewModelStore;
    private readonly DataStore _dataStore;
    private readonly IDocumentFolderService _documentFolderService;
    private readonly IDialogService _dialogService;

    private IServiceScope _serviceScope;

    public GuestListViewModel(
        GuestListViewModelStore guestListViewModelStore,
        Func<IServiceScope> createServiceScope,
        MainWindowViewModelStore mainViewModelStore,
        ClassListViewModelStore classListViewModelStore,
        CompanyListViewModelStore companyListViewModelStore,
        IDataHelperSevice dataHelperSevice,
        GuestDetailsViewModelStore guestDetailsViewModelStore,
        DataStore dataStore,
        IDocumentFolderService documentFolderService,
        IDialogService dialogService)
    {
        _guestListViewModelStore = guestListViewModelStore;
        _createServiceScope = createServiceScope;
        _mainViewModelStore = mainViewModelStore;
        _classListViewModelStore = classListViewModelStore;
        _companyListViewModelStore = companyListViewModelStore;
        _dataHelperSevice = dataHelperSevice;
        _guestDetailsViewModelStore = guestDetailsViewModelStore;
        _dataStore = dataStore;
        _documentFolderService = documentFolderService;
        _dialogService = dialogService;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        Classes = await _dataStore.GetClassesAsync();

        if (Classes is null)
        {
            return;
        }

        Companies = await _dataStore.GetCompaniesAsync();

        if (Companies is null)
        {
            return;
        }

        Guests = await _dataStore.GetGuestsAsync(true);

        if (Guests is null)
        {
            return;
        }

        _classNames = _dataHelperSevice.GetNames(Classes);
        _companyNames = _dataHelperSevice.GetNames(Companies);
        GuestList.Source = Guests;

        Filter = new GuestListFilter(
            _guestListViewModelStore,
            GuestList.View.Refresh,
            _classNames.ConvertAll(n => n),
            _companyNames.ConvertAll(n => n));

        GuestList.Filter += Filter.OnFilter;

        _mainViewModelStore.Modal.Close();
    }

    [RelayCommand]
    private void ShowGuestForm(Guest? editGuest)
    {
        _serviceScope = _createServiceScope();
        var form = (GuestInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestInputFormViewModel));
        form.EditEntity = editGuest;

        if (form.IsNewEntity)
        {
            form.SaveCompleted = (guest, formWhileSaving) => Guests.Add(guest);

            if (!string.IsNullOrEmpty(Filter?.ClassName))
            {
                form.Class = Classes?.First(c => c.Name == Filter.ClassName);
            }

        }
        else
        {
            form.SaveCompleted = (guest, formWhileSaving) => GuestList.View.Refresh();
        }

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowDeleteMessage(Guest? deleteGuest)
    {
        if (deleteGuest is null)
        {
            throw new ArgumentNullException(nameof(deleteGuest));
        }

        _serviceScope = _createServiceScope();
        var form = (GuestDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestDeleteFormViewModel));
        form.DeleteMessage = "Möchten sie den Gast und alle seine Daten (einschließlich seiner Dokumente) wirklich löschen?";
        form.DeleteEntity = deleteGuest;
        form.DeleteCompleted = () =>
        {
            DeleteGuestDocumentFolder(deleteGuest);
            Guests!.Remove(deleteGuest);
        };
        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    private void DeleteGuestDocumentFolder(Guest guest)
    {
        string? errorMsg = _documentFolderService.DeleteFolder(guest.Id.ToString());
        if (errorMsg is not null)
        {
            _dialogService.ShowMessageBox(
                $"Fehler beim Löschen des Dokumenten-Ordners '{guest.Id}'.\nBitte machen Sie ein Screenshot von dieser Meldung!\n\n{errorMsg}",
                "Fehler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
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
    private void ShowGuestDetails(Guest guest)
    {
        _guestDetailsViewModelStore.GuestParameter = guest;
        _mainViewModelStore.Sub.NavigateTo(typeof(GuestDetailsViewModel));
    }

    [RelayCommand]
    private void ShowClass(Guest guest)
    {
        if (guest.Class is null)
        {
            return;
        }

        _classListViewModelStore.ClearFilter();
        _classListViewModelStore.FilterName = guest.Class.Name;
        _mainViewModelStore.Main.NavigateTo(typeof(ClassListViewModel));
    }

    [RelayCommand]
    private void ShowCompany(Guest guest)
    {
        if (guest.Company is null)
        {
            return;
        }

        _companyListViewModelStore.ClearFilter();
        _companyListViewModelStore.FilterName = guest.Company.Name;
        _mainViewModelStore.Main.NavigateTo(typeof(CompanyListViewModel));
    }

    public override void Dispose()
    {
        if (Filter is not null)
        {
            Filter.Refresh -= GuestList.View.Refresh;
            Filter.Dispose();
        }
        base.Dispose();
    }
}