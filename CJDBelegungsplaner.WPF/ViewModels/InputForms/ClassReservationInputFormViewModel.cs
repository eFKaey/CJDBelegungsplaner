using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CJDBelegungsplaner.Domain.Services.Interfaces;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using System.Collections.Generic;
using Utility.ValidationAttributes;
using System.Linq;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class ClassReservationInputFormViewModel : InputFormBase<ClassReservation>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(Reservations))]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    private Class? _class;

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
    private ObservableCollection<Class>? _classes;

    public IEnumerable<Reservation>? Reservations => Class?.Reservations;

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

            Class = EditEntity.Class;
            Begin = EditEntity.Begin.ToString("d");
            End = EditEntity.End.ToString("d");
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

    private readonly IClassDataService _classDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly DataStore _dataStore;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly IDialogService _dialogService;

    private IServiceScope _serviceScope;

    public ClassReservationInputFormViewModel(
        IClassDataService classDataService,
        IHandleResultService handleResultService,
        IAccountService accountService,
        DataStore dataStore,
        Func<IServiceScope> createServiceScope,
        IDialogService dialogService)
    {
        _classDataService = classDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
        _dataStore = dataStore;
        _createServiceScope = createServiceScope;
        _dialogService = dialogService;

        _classes = _dataStore.GetClasses(true);
    }

    #endregion

    public override async Task<(bool, ClassReservation)> CreateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<ClassDataServiceResultKind, Class> result;

        ClassReservation reservation = new ClassReservation
        {
            Begin = DateTime.Parse(Begin!),
            End = DateTime.Parse(End!),
            Class = this.Class
        };

        Interval? overlap = reservation.GetFirstOverlappingInterval(Class.Reservations);
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bereits existierenden Reservierung ({overlap.DateRangeFormatted}) dieser Klasse festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            return (isFailure, reservation);
        }

        Class!.Reservations.Add(reservation);

        result = await Task.Run(() => _classDataService.UpdateAsync(Class!.Id, Class));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            Class.Reservations.Remove(reservation);
            return (isFailure, reservation);
        }

        _accountService.MakeUserLogEntry($"Klasse '{Class.Name}' Reservierung {Begin} - {End} hinzugefügt.");

        return (isSuccess, reservation);
    }

    public override async Task<(bool, ClassReservation)> UpdateAsync()
    {
        bool isSuccess = false;
        bool isFailure = true;

        Result<ClassDataServiceResultKind, Class> result;

        ClassReservation tempReservation = EditEntity!.Clone();

        EditEntity.Begin = DateTime.Parse(Begin!);
        EditEntity.End = DateTime.Parse(End!);

        List<Interval> intervals = new();
        foreach (var guest in EditEntity.Guests)
        {
            intervals = intervals.Concat(guest.Reservations).ToList();
        }
        Interval? overlap = EditEntity.GetFirstOverlappingInterval(Class.Reservations.OfType<Interval>().Concat(intervals));
        if (overlap is not null)
        {
            _dialogService.ShowMessageDialog($"Es wurde eine Überschneidung mit einer bereits existierenden Reservierung ({overlap.DateRangeFormatted}) dieser Klasse oder einem dieser Klassen-Reservierung zugewiesenen Gast festgestellt.", "Überschneidung", MessageBoxImage.Exclamation);
            tempReservation.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        result = await Task.Run(() => _classDataService.UpdateAsync(Class.Id, Class));

        isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempReservation.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Klasse '{Class.Name}' Reservierung {Begin} - {End} bearbeitet.");

        return (isSuccess, EditEntity);
    }

    [RelayCommand]
    private void ShowClassForm()
    {
        if (Classes is null)
        {
            throw new NullReferenceException(nameof(Classes));
        }

        _serviceScope = _createServiceScope();
        var form = (ClassInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassInputFormViewModel));
        form.SaveCompleted = (@class, formWhileSaving) =>
        {
            Classes.Add(@class);
            Class = @class;
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
}
