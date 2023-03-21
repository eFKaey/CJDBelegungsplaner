using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

public partial class ReservationInputFormViewModel : InputFormBase
{
    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    private ClassReservationInputFormViewModel? _classReservationForm;
    private GuestReservationInputFormViewModel? _guestReservationForm;

    public string? Begin { get; set; }
    public string? End { get; set; }

    public EntityObject EditReservation { get; set; }
    public bool IsNewEntity => EditReservation is null;

    public bool IsClassFormButtonChecked { get; set; } = true;
    public bool IsGuestFormButtonChecked { get; set; } = false;
    public bool IsClassFormButtonEnabled { get; set; } = true;
    public bool IsGuestFormButtonEnabled { get; set; } = true;

    /// <summary>
    /// Wird am Ende des Speicher/Update-Vorgangs ausgeführt.
    /// </summary>
    public Action<ClassReservation>? SaveClassReservationCompleted;

    /// <summary>
    /// Wird am Ende des Speicher/Update-Vorgangs ausgeführt.
    /// </summary>
    public Action<GuestReservation>? SaveGuestReservationCompleted;

    private IServiceScope _serviceScope;

    public ReservationInputFormViewModel(Func<IServiceScope> createServiceScope)
    {
        _serviceScope = createServiceScope();
    }

    [RelayCommand]
    private void ShowClassReservationForm()
    {
        if (_classReservationForm is null)
        {
            _classReservationForm = (ClassReservationInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassReservationInputFormViewModel));
            _classReservationForm.Begin = Begin;
            _classReservationForm.End = End;
            _classReservationForm.SaveCompleted = SaveClassReservationCompleted;
            _classReservationForm.ExecuteClose = ExecuteClose;
            if (EditReservation != null)
            {
                _classReservationForm.EditEntity = (ClassReservation)EditReservation;
            }
        }
        CurrentFormViewModel = _classReservationForm;
    }

    [RelayCommand]
    private void ShowGuestReservationForm()
    {
        if (_guestReservationForm is null)
        {
            _guestReservationForm = (GuestReservationInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestReservationInputFormViewModel));
            _guestReservationForm.Begin = Begin;
            _guestReservationForm.End = End;
            _guestReservationForm.SaveCompleted = SaveGuestReservationCompleted;
            _guestReservationForm.ExecuteClose = ExecuteClose;
            if (EditReservation != null)
            {
                _guestReservationForm.EditEntity = (GuestReservation)EditReservation;
            }
        }
        CurrentFormViewModel = _guestReservationForm;
    }

    public override void Dispose()
    {
        CurrentFormViewModel = null;
        _classReservationForm = null;
        _guestReservationForm = null;
        _serviceScope?.Dispose();
        SaveClassReservationCompleted = null;
        SaveGuestReservationCompleted = null;
        base.Dispose();
    }
}
