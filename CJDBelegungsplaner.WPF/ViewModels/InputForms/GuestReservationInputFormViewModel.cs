using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class GuestReservationInputFormViewModel : InputFormBase<GuestReservation>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    private Guest? _guest;

    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [ObservableProperty]
    private string? _begin = string.Empty;

    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [ObservableProperty]
    private string? _end = string.Empty;

    #endregion

    #region Data Fields/Properies

    [ObservableProperty]
    private ObservableCollection<Guest>? _guests;

    public override GuestReservation? EditEntity
    {
        get { return base.EditEntity; }
        set
        {
            base.EditEntity = value;

            if (EditEntity is null)
            {
                return;
            }

            Guest = EditEntity.Guest;
            Begin = EditEntity.Begin.ToString("d");
            End = EditEntity.End.ToString("d");
        }
    }

    #endregion

    #region Dialog Fields/Properties

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private GuestInputFormViewModel _guestForm;

    [ObservableProperty]
    private bool _isDialogOpen;

    #endregion

    #region Injection Fields and Constructor

    private readonly IGuestDataService _guestDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly DataStore _dataStore;
    private readonly Func<IServiceScope> _createServiceScope;

    private IServiceScope _serviceScope;

    public GuestReservationInputFormViewModel(
        IGuestDataService guestDataService,
        IHandleResultService handleResultService,
        IAccountService accountService,
        DataStore dataStore,
        Func<IServiceScope> createServiceScope)
    {
        _guestDataService = guestDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
        _dataStore = dataStore;
        _createServiceScope = createServiceScope;

        Guests = _dataStore.GetGuests();
    }

    #endregion

    public override async Task<(bool, GuestReservation)> CreateAsync()
    {
        bool isSuccess = false;

        Result<DataServiceResultKind, Guest> result;

        Guest guest = Guest!;

        GuestReservation reservation = new GuestReservation
        {
            Begin = DateTime.Parse(Begin!),
            End = DateTime.Parse(End!),
            Guest = Guest
        };

        guest.Reservations.Add(reservation);

        result = await Task.Run(() => _guestDataService.UpdateAsync(guest!.Id, guest));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return (isFailure, reservation);
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) '{guest.Name}' Reservierung {Begin} - {End} hinzugefügt.");

        return (isSuccess, reservation);
    }

    public override async Task<(bool, GuestReservation)> UpdateAsync()
    {
        bool isSuccess = false;

        Result<DataServiceResultKind, Guest> result;

        GuestReservation tempReservation = EditEntity!.Clone();

        EditEntity.Begin = DateTime.Parse(Begin!);
        EditEntity.End = DateTime.Parse(End!);

        Guest guest = EditEntity.Guest;

        result = await Task.Run(() => _guestDataService.UpdateAsync(guest.Id, guest));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempReservation.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) '{guest.Name}' Reservierung {Begin} - {End} bearbeitet.");

        return (isSuccess, EditEntity);
    }

    [RelayCommand]
    private void ShowGuestForm()
    {
        if (Guests is null)
        {
            throw new NullReferenceException(nameof(Guests));
        }

        _serviceScope = _createServiceScope();
        GuestForm = (GuestInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestInputFormViewModel));
        GuestForm.SaveCompleted = (guest) =>
        {
            Guests.Add(guest);
            Guest = guest;
        };
        GuestForm.ExecuteClose = CloseDialog;
        CurrentFormViewModel = GuestForm;

        OpenDialog();
    }

    private void OpenDialog()
    {
        IsDialogOpen = true;
    }

    [RelayCommand]
    private void CloseDialog()
    {
        IsDialogOpen = false;
        CurrentFormViewModel = null;
        _serviceScope?.Dispose();
    }
}
