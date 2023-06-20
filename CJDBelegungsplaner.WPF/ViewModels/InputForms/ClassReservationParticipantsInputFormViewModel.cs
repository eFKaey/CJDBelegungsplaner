using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CJDBelegungsplaner.WPF.Intermediate;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class ClassReservationParticipantsInputFormViewModel : InputFormBase<ClassReservation>
{
    #region Data Fields/Properies

    [ObservableProperty]
    public ObservableCollection<CheckableEntityObject<Guest>> _checkableGuests = new ObservableCollection<CheckableEntityObject<Guest>>();

    public override ClassReservation? EditEntity
    {
        get { return base.EditEntity; }
        set
        {
            base.EditEntity = value;

            if (EditEntity is null)
            {
                return;
            }

            foreach (var guest in EditEntity.Class.Guests)
            {
                CheckableGuests.Add(new CheckableEntityObject<Guest>(guest, EditEntity.Guests.Contains(guest)));
            }
        }
    }

    [ObservableProperty]
    private bool _areAllChecked = false;

    #endregion

    #region Dialog Fields/Properties

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    #endregion

    #region Injection Fields and Constructor

    private readonly IClassDataService _classDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly IDialogService _dialogService;

    private IServiceScope _serviceScope;

    public ClassReservationParticipantsInputFormViewModel(
        IClassDataService classDataService,
        IHandleResultService handleResultService,
        IAccountService accountService,
        Func<IServiceScope> createServiceScope,
        IDialogService dialogService)
    {
        _classDataService = classDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
        _createServiceScope = createServiceScope;
        _dialogService = dialogService;
    }

    #endregion

    public override Task<(bool, ClassReservation)> CreateAsync()
    {
        throw new NotImplementedException();
    }

    public override async Task<(bool, ClassReservation)> UpdateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<ClassDataServiceResultKind, Class> result;

        ClassReservation reservation = EditEntity!;

        List<Guest> tempGuests = reservation.Guests.ToList();

        ToggleReservationParticipation(reservation);

        Class @class = reservation.Class;

        List<Interval> intervals = new();
        foreach (var guest in reservation.Guests)
        {
            intervals = intervals.Concat(guest.Reservations).ToList();
        }
        Interval? overlap = reservation.GetFirstOverlappingInterval(intervals);
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bereits existierenden Gast-Reservierung ({overlap.DateRangeFormatted}) festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            ResetCheckableGuests(tempGuests);
            ToggleReservationParticipation(reservation);
            return (isFailure, reservation);
        }

        result = await Task.Run(() => _classDataService.UpdateAsync(@class.Id, @class));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            ResetCheckableGuests(tempGuests);
            ToggleReservationParticipation(reservation);
            return (isFailure, reservation);
        }

        _accountService.MakeUserLogEntry($"Teilnehmerauswahl der Reservierung {reservation.DateRangeFormatted} der Klasse '{@class.Name}' mit nun {reservation.ParticipantsCount} Teilnehmern bearbeitet.");

        return (isSuccess, reservation);
    }

    private void ResetCheckableGuests(List<Guest> tempGuests)
    {
        foreach (CheckableEntityObject<Guest> checkableGuest in CheckableGuests)
        {
            checkableGuest.IsChecked = false;

            foreach (Guest tempGuest in tempGuests)
            {
                if (checkableGuest.Entity == tempGuest)
                {
                    checkableGuest.IsChecked = true;
                }
            }
        }
    }

    private void ToggleReservationParticipation(ClassReservation reservation)
    {
        Guest guest;

        foreach (CheckableEntityObject<Guest> checkableGuest in CheckableGuests)
        {
            guest = checkableGuest.Entity;

            if (checkableGuest.IsChecked == false && reservation.Guests.Contains(guest))
            {
                guest.ClassReservations.Remove(reservation);
                reservation.Guests.Remove(guest);
            }
            else if (checkableGuest.IsChecked == true && ! reservation.Guests.Contains(guest))
            {
                guest.ClassReservations.Add(reservation);
                reservation.Guests.Add(guest);
            }
        }
    }

    [RelayCommand]
    private void ToggleCheckboxes()
    {
        if (AreAllChecked)
        {
            foreach (CheckableEntityObject<Guest> checkableGuest in CheckableGuests)
            {
                checkableGuest.IsChecked = true;
            }
            return;
        }

        foreach (CheckableEntityObject<Guest> checkableGuest in CheckableGuests)
        {
            checkableGuest.IsChecked = false;
        }
    }

    [RelayCommand]
    private void InvertCheckboxes()
    {
        foreach (CheckableEntityObject<Guest> checkableGuest in CheckableGuests)
        {
            checkableGuest.IsChecked = checkableGuest.IsChecked ? false : true;
        }
    }

    [RelayCommand]
    private void ShowGuestForm(Guest? editGuest)
    {
        if (EditEntity is null)
        {
            throw new NullReferenceException(nameof(EditEntity));
        }
        if (EditEntity.Class is null)
        {
            throw new NullReferenceException(nameof(EditEntity.Class));
        }

        Class @class = EditEntity.Class;

        _serviceScope = _createServiceScope();
        var form = (GuestInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestInputFormViewModel));
        form.EditEntity = editGuest;
        form.Class = @class;
        form.IsClassInputEnabled = false;
        if (form.IsNewEntity)
        {

            form.SaveCompleted = (guest, formAfterSave) =>
                CheckableGuests.Add(new CheckableEntityObject<Guest>(guest, true));
        }
        else
        {
            form.SaveCompleted = (guest, formWhileSaving) =>
            {
                (CheckableGuests, var temp) = (null, CheckableGuests);
                CheckableGuests = temp;
            };
        }
        form.ExecuteClose = CloseDialog;
        CurrentFormViewModel = form;

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