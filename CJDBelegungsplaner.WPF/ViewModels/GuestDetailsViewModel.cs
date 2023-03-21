using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Filter;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class GuestDetailsViewModel : ViewModelBase
{
    #region Properties/Fields

    private readonly GuestDetailsViewModelStore _guestDetailsViewModelStore;
    private readonly MainWindowViewModelStore _mainViewModelStore;

    [ObservableProperty]
    private Guest _guest;

    [ObservableProperty]
    private CollectionViewSource _reservationList = new CollectionViewSource();

    #endregion

    #region Konstruktor

    public GuestDetailsViewModel(
        GuestDetailsViewModelStore guestDetailsViewModelStore, 
        MainWindowViewModelStore mainViewModelStore)
    {
        _guestDetailsViewModelStore = guestDetailsViewModelStore;
        _mainViewModelStore = mainViewModelStore;

        LoadDataAsync();
    }

    #endregion

    private async Task LoadDataAsync()
    {
        if (_guestDetailsViewModelStore.IsGuestParameter)
        {
            Guest = _guestDetailsViewModelStore.GuestParameter!;
        }

        if (Guest is null)
        {
            return;
        }

        if (Guest.Reservations is not null)
        {
            ReservationList.Source = Guest.Reservations;
        }

        _mainViewModelStore.Modal.Close();
    }
}
