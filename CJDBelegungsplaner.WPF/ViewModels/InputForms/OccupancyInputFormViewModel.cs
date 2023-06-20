using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Utility.ValidationAttributes;
using System.Windows.Media;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class OccupancyInputFormViewModel : InputFormBase<Occupancy>
{
    #region Input Fields/Properies

    private Class? _class;
    public Class? Class
    {
        get { return _class; }
        set
        {
            _class = value;

            if (_class is not null)
            {
                GuestList.Filter += _filterWithSpecificClass;
                GuestList.View.Refresh();
            }

            OnPropertyChanged(nameof(Class));
        }
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(Reservations))]
    [NotifyPropertyChangedFor(nameof(Occupancies))]
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

    [ObservableProperty]
    private Color _color = Color.FromRgb(235, 64, 52);

    [ObservableProperty]
    private string? _information;

    private Reservation? _selectedReservation;
    public Reservation? SelectedReservation
    {
        get { return _selectedReservation; }
        set
        {
            _selectedReservation = value;

            if (_selectedReservation is not null)
            {
                Begin = DateTime.MinValue.ToString("d");
                End = DateTime.MaxValue.ToString("d");
                Begin = _selectedReservation.Begin.ToString("d");
                End = _selectedReservation.End.ToString("d");
            }

            OnPropertyChanged(nameof(SelectedReservation));
        }
    }

    #endregion

    #region Data Fields/Properies

    [ObservableProperty]
    private ObservableCollection<Guest>? _guests;

    public IEnumerable<Reservation>? Reservations => Guest?.ClassReservations.OfType<Reservation>().Concat(Guest.Reservations);
    public IEnumerable<Occupancy>? Occupancies => Guest?.Occupancies;    

    [ObservableProperty]
    private CollectionViewSource _guestList = new CollectionViewSource();

    [ObservableProperty]
    private ObservableCollection<Class>? _classes;

    public override Occupancy? EditEntity
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
            Color = EditEntity.Color;
            Information = EditEntity.Information;
        }
    }

    public Bed Bed { get; set; }

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

    private FilterEventHandler _filterNoClass;
    private FilterEventHandler _filterWithClasses;
    private FilterEventHandler _filterWithSpecificClass;

    private void InitializeFilter()
    {
        _filterNoClass = (object sender, FilterEventArgs e) => e.Accepted = (e.Item as Guest).Class is null;
        _filterWithClasses = (object sender, FilterEventArgs e) => e.Accepted = (e.Item as Guest).Class is not null;
        _filterWithSpecificClass = (object sender, FilterEventArgs e) => e.Accepted = (e.Item as Guest).Class == Class;
    }

    private bool? _isShowGuestsWithClass = false;
    public bool? IsShowGuestsWithClass
    {
        get { return _isShowGuestsWithClass; }
        set
        {
            GuestList.View.Filter = null;

            if (_isShowGuestsWithClass == true)
            {
                Class = null;
                _isShowGuestsWithClass = false;
                GuestList.Filter += _filterNoClass;
            }
            else if (_isShowGuestsWithClass == false)
            {
                Class = null;
                _isShowGuestsWithClass = null;
            }
            else if (_isShowGuestsWithClass == null)
            {
                Class = null;
                _isShowGuestsWithClass = true;
                GuestList.Filter += _filterWithClasses;
            }

            OnPropertyChanged(nameof(IsShowGuestsWithClass));
            OnPropertyChanged(nameof(ShowGuestsWithClassText));
            GuestList.View.Refresh();
        }
    }

    #endregion

    #region Injection Fields and Constructor

    private readonly IBedDataService _bedDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly DataStore _dataStore;
    private readonly IDialogService _dialogService;

    public OccupancyInputFormViewModel(
        IBedDataService bedDataService,
        IHandleResultService handleResultService,
        IAccountService accountService,
        DataStore dataStore,
        IDialogService dialogService)
    {
        _bedDataService = bedDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
        _dataStore = dataStore;
        _dialogService = dialogService;

        InitializeFilter();

        _classes = _dataStore.GetClasses(true);

        _guests = _dataStore.GetGuests(true);
        GuestList.Source = _guests;
        GuestList.Filter += _filterNoClass;
    }

    #endregion

    public override async Task<(bool, Occupancy)> CreateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<BedDataServiceResultKind, Bed> result;

        Occupancy occupancy = new()
        {
            Begin = DateTime.Parse(Begin!),
            End = DateTime.Parse(End!),
            Guest = Guest,
            Color = Color,
            Information = Information
        };

        Interval? overlap = occupancy.GetFirstOverlappingInterval(Bed.Occupancies);
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Der Gast hat in diesem Zeitraum bereits eine Belegung beim Bett {(overlap as Occupancy).Bed.Name}.", "Überschneidung", MessageBoxImage.Exclamation);
            return (isFailure, occupancy);
        }

        overlap = occupancy.GetFirstOverlappingInterval(Guest.Occupancies);
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bestehenden Belegung ({overlap.DateRangeFormatted}) festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            return (isFailure, occupancy);
        }

        Bed.Occupancies.Add(occupancy);

        result = await Task.Run(() => _bedDataService.UpdateAsync(Bed.Id, Bed));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            Bed.Occupancies.Remove(occupancy);
            return (isFailure, null)!;
        }

        _accountService.MakeUserLogEntry($"Bett '{Bed.Name}' Belegung {Begin} - {End} für Gast '{Guest.Name}' hinzugefügt.", Guest);

        return (isSuccess, occupancy);
    }

    public override async Task<(bool, Occupancy)> UpdateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<BedDataServiceResultKind, Bed> result;

        Occupancy tempOccupancy = EditEntity!.Clone();

        EditEntity.Begin = DateTime.Parse(Begin!);
        EditEntity.End = DateTime.Parse(End!);
        EditEntity.Color = Color;
        EditEntity.Information = Information;

        Interval? overlap = EditEntity.GetFirstOverlappingInterval(Bed.Occupancies);
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Der Gast hat in diesem Zeitraum bereits eine Belegung beim Bett {(overlap as Occupancy).Bed.Name}.", "Überschneidung", MessageBoxImage.Exclamation);
            tempOccupancy.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        overlap = EditEntity.GetFirstOverlappingInterval(Guest.Occupancies);
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bestehenden Belegung ({overlap.DateRangeFormatted}) festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            tempOccupancy.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        result = await Task.Run(() => _bedDataService.UpdateAsync(Bed.Id, Bed));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempOccupancy.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Bett '{Bed.Name}' Belegung {Begin} - {End} für Gast '{Guest.Name}' bearbeitet.", Guest);

        return (isSuccess, EditEntity);
    }

    public override void Dispose()
    {
        GuestList.View.Filter = null;
        base.Dispose();
    }
}
