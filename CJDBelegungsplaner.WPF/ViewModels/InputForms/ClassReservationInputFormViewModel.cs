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

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class ClassReservationInputFormViewModel : InputFormBase<ClassReservation>
{
    #region Input Fields/Properies

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Muss ausgefüllt werden!")]
    private Class? _class;

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
    private ObservableCollection<Class>? _classes;

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
    private ClassInputFormViewModel _classForm;

    [ObservableProperty]
    private bool _isDialogOpen;

    #endregion

    #region Injection Fields and Constructor

    private readonly IClassDataService _classDataService;
    private readonly IHandleResultService _handleResultService;
    private readonly IAccountService _accountService;
    private readonly DataStore _dataStore;
    private readonly Func<IServiceScope> _createServiceScope;

    private IServiceScope _serviceScope;

    public ClassReservationInputFormViewModel(
        IClassDataService classDataService,
        IHandleResultService handleResultService,
        IAccountService accountService,
        DataStore dataStore,
        Func<IServiceScope> createServiceScope)
    {
        _classDataService = classDataService;
        _handleResultService = handleResultService;
        _accountService = accountService;
        _dataStore = dataStore;
        _createServiceScope = createServiceScope;

        Classes = _dataStore.GetClasses();
    }

    #endregion

    public override async Task<(bool, ClassReservation)> CreateAsync()
    {
        bool isSuccess = false;

        Result<ClassDataServiceResultKind, Class> result;

        Class @class = Class;

        ClassReservation reservation = new ClassReservation
        {
            Begin = DateTime.Parse(Begin!),
            End = DateTime.Parse(End!),
            Class = Class
        };

        @class.Reservations.Add(reservation);

        foreach (Guest guest in @class.Guests)
        {
            guest.ClassReservations.Add(reservation);
            reservation.Guests.Add(guest);
        }

        result = await Task.Run(() => _classDataService.UpdateAsync(@class!.Id, @class));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            return (isFailure, reservation);
        }

        _accountService.MakeUserLogEntry($"Klasse '{@class.Name}' Reservierung {Begin} - {End} hinzugefügt.");

        return (isSuccess, reservation);
    }

    public override async Task<(bool, ClassReservation)> UpdateAsync()
    {
        bool isSuccess = false;
        
        Result<ClassDataServiceResultKind, Class> result;

        ClassReservation tempReservation = EditEntity!.Clone();

        EditEntity.Begin = DateTime.Parse(Begin!);
        EditEntity.End = DateTime.Parse(End!);

        Class @class = EditEntity.Class;

        result = await Task.Run(() => _classDataService.UpdateAsync(@class.Id, @class));

        bool isFailure = _handleResultService.Handle(result);

        if (isFailure)
        {
            tempReservation.CopyValuesTo(EditEntity);
            return (isFailure, EditEntity);
        }

        _accountService.MakeUserLogEntry($"Klasse '{@class.Name}' Reservierung {Begin} - {End} bearbeitet.");

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
        ClassForm = (ClassInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassInputFormViewModel));
        ClassForm.SaveCompleted = (@class) =>
        {
            Classes.Add(@class);
            Class = @class;
        };
        ClassForm.ExecuteClose = CloseDialog;
        CurrentFormViewModel = ClassForm;

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
