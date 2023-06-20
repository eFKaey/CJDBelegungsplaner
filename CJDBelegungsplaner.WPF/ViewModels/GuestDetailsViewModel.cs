using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class GuestDetailsViewModel : ViewModelBase
{
    #region Properties/Fields

    private readonly GuestDetailsViewModelStore _guestDetailsViewModelStore;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly IDocumentFolderService _documentFolderService;

    [ObservableProperty]
    private Guest _guest;

    #endregion

    #region Konstruktor

    public GuestDetailsViewModel(
        GuestDetailsViewModelStore guestDetailsViewModelStore,
        MainWindowViewModelStore mainViewModelStore,
        IDocumentFolderService documentFolderService)
    {
        _guestDetailsViewModelStore = guestDetailsViewModelStore;
        _mainViewModelStore = mainViewModelStore;
        _documentFolderService = documentFolderService;

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        if (_guestDetailsViewModelStore.IsGuestParameter)
        {
            Guest = _guestDetailsViewModelStore.GuestParameter!;
        }

        _mainViewModelStore.Modal.Close();
    }

    [RelayCommand]
    private void ShowDocuments()
    {
        _documentFolderService.OpenFolderInFileExplorer(Guest.Id.ToString());
    }
}
