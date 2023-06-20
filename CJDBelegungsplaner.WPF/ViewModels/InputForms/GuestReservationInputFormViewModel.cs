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
using System.Windows.Data;
using Utility.ValidationAttributes;
using System.Linq;
using System.Collections.Generic;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class GuestReservationInputFormViewModel : InputFormBase<GuestReservation>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(Reservations))]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    private Guest? _guest;


    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [DateRange(nameof(End), DateRangeAttribute.MustBe.GreaterThanOrEqual, ErrorMessage = "Muss kleiner als Enddatum sein!")]
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(End))]
    private string? _begin = DateTime.MinValue.ToString("d");

    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    [DateRange(nameof(Begin), DateRangeAttribute.MustBe.LesserThanOrEqual, ErrorMessage = "Muss größer als Startdatum sein!")]
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Begin))]
    private string? _end = DateTime.MaxValue.ToString("d");

    #endregion

    #region Data Fields/Properies

    [ObservableProperty]
    private ObservableCollection<Guest>? _guests;

    [ObservableProperty]
    private CollectionViewSource _guestList = new CollectionViewSource();

    public IEnumerable<Reservation>? Reservations => Guest?.ClassReservations.OfType<Reservation>().Concat(Guest.Reservations);

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

            GuestList.View.Filter = null;
            Guest = EditEntity.Guest;
            Begin = EditEntity.Begin.ToString("d");
            End = EditEntity.End.ToString("d");
        }
    }

    public string ShowGuestsWithClassText
    {
        get
        {
            if (_isShowGuestsWithClass == true)
            {
                return "Gäste mit Klasse werden angezeigt.";
            }
            else if (_isShowGuestsWithClass == false)
            {
                return "Gäste ohne Klasse werden angezeigt.";
            }
            return "Alle Gäste werden angezeigt.";
        }
    }

    private FilterEventHandler _filterNoClass = (object sender, FilterEventArgs e) => e.Accepted = (e.Item as Guest).Class is null;
    private FilterEventHandler _filterWithClass = (object sender, FilterEventArgs e) => e.Accepted = (e.Item as Guest).Class is not null;

    private bool? _isShowGuestsWithClass = false;
    public bool? IsShowGuestsWithClass
    {
        get { return _isShowGuestsWithClass; }
        set
        {
            GuestList.View.Filter = null;

            if (_isShowGuestsWithClass == true)
            {
                _isShowGuestsWithClass = false;
                GuestList.Filter += _filterNoClass;
            }
            else if (_isShowGuestsWithClass == false)
            {
                _isShowGuestsWithClass = null;
            }
            else if (_isShowGuestsWithClass == null)
            {
                _isShowGuestsWithClass = true;
                GuestList.Filter += _filterWithClass;
            }

            OnPropertyChanged(nameof(IsShowGuestsWithClass));
            OnPropertyChanged(nameof(ShowGuestsWithClassText));
            GuestList.View.Refresh();
        }
    }

    #endregion

    #region Dialog Fields/Properties

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    #endregion

    #region Injection Fields and Constructor

    private readonly IGuestDataService _guestDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly DataStore _dataStore;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly IDialogService _dialogService;

    private IServiceScope _serviceScope;

    public GuestReservationInputFormViewModel(
        IGuestDataService guestDataService,
        IHandleResultService handleResultService,
        IAccountService accountService,
        DataStore dataStore,
        Func<IServiceScope> createServiceScope,
        IDialogService dialogService)
    {
        _guestDataService = guestDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
        _dataStore = dataStore;
        _createServiceScope = createServiceScope;
        _dialogService = dialogService;

        _guests = _dataStore.GetGuests(true);
        GuestList.Source = _guests;
        GuestList.Filter += _filterNoClass;
    }

    #endregion

    public override async Task<(bool, GuestReservation)> CreateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<DataServiceResultKind, Guest> result;

        GuestReservation reservation = new GuestReservation
        {
            Begin = DateTime.Parse(Begin!),
            End = DateTime.Parse(End!),
            Guest = this.Guest
        };

        Interval? overlap = reservation.GetFirstOverlappingInterval(Guest.Reservations.OfType<Interval>().Concat(Guest.ClassReservations));
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bereits existierenden Reservierung ({overlap.DateRangeFormatted}) festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            return (isFailure, reservation);
        }

        Guest!.Reservations.Add(reservation);

        result = await Task.Run(() => _guestDataService.UpdateAsync(Guest!.Id, Guest));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            Guest.Reservations.Remove(reservation);
            return (isFailure, reservation);
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) '{Guest.Name}' Reservierung {Begin} - {End} hinzugefügt.", Guest);

        return (isSuccess, reservation);
    }

    public override async Task<(bool, GuestReservation)> UpdateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<DataServiceResultKind, Guest> result;

        GuestReservation tempReservation = EditEntity!.Clone();

        EditEntity.Begin = DateTime.Parse(Begin!);
        EditEntity.End = DateTime.Parse(End!);

        Interval? overlap = EditEntity.GetFirstOverlappingInterval(Guest.Reservations.OfType<Interval>().Concat(Guest.ClassReservations));
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bereits existierenden Reservierung ({overlap.DateRangeFormatted}) festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            tempReservation.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        result = await Task.Run(() => _guestDataService.UpdateAsync(Guest.Id, Guest));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempReservation.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Gast(Schüler) '{Guest.Name}' Reservierung {Begin} - {End} bearbeitet.", Guest);

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
        var form = (GuestInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestInputFormViewModel));
        form.SaveCompleted = (guest, formWhileSaving) =>
        {
            Guests.Add(guest);
            Guest = guest;
        };
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

    public override void Dispose()
    {
        GuestList.View.Filter = null;
        base.Dispose();
    }
}
