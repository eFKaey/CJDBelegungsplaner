using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;
using System.Collections.Generic;
using System.Linq;
using CJDBelegungsplaner.WPF.Intermediate.OccupancyList;
using CJDBelegungsplaner.WPF.Services.Interfaces;
using System.Windows;
using System.Threading;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class OccupancyViewModel : ViewModelBase
{
    #region Properties/Fields

    [ObservableProperty]
    private string _startDate;

    public int[] MonthAmountSource { get; set; } = new int[] { 1, 2, 3, 4, 5, 6 };

    [ObservableProperty]
    private int _monthAmount;

    private DateTime _begin => DateTime.Parse(StartDate);
    private DateTime _end => _begin.AddMonths(MonthAmount);

    [ObservableProperty]
    private ObservableCollection<Bed>? _beds;
    [ObservableProperty]
    private ObservableCollection<BedRow>? _bedRows;

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    public bool HasPermissionFoEditing => _accountService.HasEqualOrHigherPermissionFor(Role.Standard);

    #endregion

    #region Konstruktor

    private readonly Func<IServiceScope> _createServiceScope;
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly DataStore _dataStore;
    private readonly IAccountService _accountService;
    private readonly GuestDetailsViewModelStore _guestDetailsViewModelStore;

    private IServiceScope _serviceScope;

    public OccupancyViewModel(
        MainWindowViewModelStore mainViewModelStore,
        Func<IServiceScope> createServiceScope,
        DataStore dataStore,
        IAccountService accountService,
        GuestDetailsViewModelStore guestDetailsViewModelStore)
    {
        _mainViewModelStore = mainViewModelStore;
        _createServiceScope = createServiceScope;
        _dataStore = dataStore;
        _accountService = accountService;
        _guestDetailsViewModelStore = guestDetailsViewModelStore;

        StartDate = DateTime.Now.AddDays(-15).ToString("d");
        MonthAmount = 2;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataCommand.Execute(null);
    }

    #endregion

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        Beds = await _dataStore.GetBedsAsync(true);

        if (Beds is null)
        {
            return;
        }

        GenerateRows();
    }

    [RelayCommand]
    private async Task GenerateRows()
    {
        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        BedRows = new();

        // Workaround, damit das WarteOverlay angezeigt, auch wenns hängt.
        await Task.Run(() => Thread.Sleep(500));

        // Bringt nicht viel. App hängt dennoch, weil die Aktualisierung der UI das Probelm ist.
        await Task.Run(() =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Bed bed in Beds)
                {
                    BedRows.Add(CreateBedRow(bed));
                }
            });
        });

        _mainViewModelStore.Modal.Close();
    }

    private BedRow CreateBedRow(Bed bed)
    {
        var begin = _begin;
        var end = _end;

        BedRow row = new BedRow { Bed = bed };
        Interval? interval = null;

        for (DateTime day = begin; day < end; day = day.AddDays(1))
        {
            if (interval is null || day < interval.Begin || interval.End < day)
            {
                interval = FindInterval(bed.Occupancies, day);

                if (interval is null)
                {
                    interval = FindInterval(bed.Unavailabilities, day);
                }
            }

            row.Days.Add(new Day
            {
                Bed = bed,
                Date = day,
                Interval = interval
            });
        }

        return row;
    }

    private static Interval? FindInterval(IEnumerable<Interval> intervals, DateTime day)
    {
        foreach (Interval interval in intervals)
        {
            if (interval.Begin <= day && day <= interval.End)
            {
                return interval;
            }
        }

        return null;
    }

    [RelayCommand]
    private void ShowBedForm(Bed? editBed)
    {
        _serviceScope = _createServiceScope();
        var form = (BedInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(BedInputFormViewModel));
        form.EditEntity = editBed;

        if (form.IsNewEntity)
        {
            form.SaveCompleted = (bed, formWhileSaving) =>
            {
                Beds.Add(bed);
                BedRows.Add(CreateBedRow(bed));
            };
        }
        else
        {
            form.SaveCompleted = (bed, formWhileSaving) =>
            {
                RefreshBedRow(bed);
            };
        }

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowOccupancyForm(Day day)
    {
        if (day is null)
        {
            throw new ArgumentNullException(nameof(day));
        }
        if (day.Interval is Unavailability)
        {
            // TODO: Dieser Hinweis sollte über den DialogService laufen! Oder Occupancy Form kann nicht ausgewählt werden, wenn eine Unavailability vorliegt.
            System.Windows.MessageBox.Show($"Schon mit ner {nameof(Unavailability)} belegt.");
            return;
        }

        _serviceScope = _createServiceScope();
        var form = (OccupancyInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(OccupancyInputFormViewModel));
        form.Begin = day.Date.ToString("d");
        form.End = day.Date.ToString("d");
        form.EditEntity = day.Interval as Occupancy;
        form.Bed = day.Bed;

        form.SaveCompleted = (occupancy, formWhileSaving) =>
        {
            RefreshBedRow(form.Bed);
        };

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    private void RefreshBedRow(Bed bed)
    {
        var bedRow = BedRows.First(row => row.Bed == bed);
        var index = BedRows.IndexOf(bedRow);
        BedRows.Insert(index, CreateBedRow(bed));
        BedRows.RemoveAt(index + 1);
    }

    [RelayCommand]
    private void ShowBedDeleteMessage(Bed deleteBed)
    {
        if (deleteBed is null)
        {
            throw new ArgumentNullException(nameof(deleteBed));
        }

        _serviceScope = _createServiceScope();
        var form = (BedDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(BedDeleteFormViewModel));
        form.DeleteMessage = "Möchten sie das Bett und alle seine Daten wirklich löschen?";
        form.DeleteEntity = deleteBed;
        form.DeleteCompleted = () =>
        {
            Beds!.Remove(deleteBed);
            var bedRow = BedRows.First(row => row.Bed == deleteBed);
            BedRows.Remove(bedRow);
        };
        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowOccupancyDeleteMessage(Day day)
    {
        if (day is null)
        {
            throw new ArgumentNullException(nameof(day));
        }

        var occupancy = day.Interval as Occupancy;

        if (occupancy is null)
        {
            throw new Exception(nameof(occupancy));
        }

        _serviceScope = _createServiceScope();
        var form = (OccupancyDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(OccupancyDeleteFormViewModel));
        form.DeleteMessage = $"Belegung vom {occupancy.Begin.ToString("d")}-{occupancy.End.ToString("d")} löschen? \n Bett: {day.Bed.Name} \n Gast: {occupancy.Guest.Name}";
        form.DeleteEntity = occupancy;
        form.Bed = day.Bed;
        form.DeleteCompleted = () =>
        {
            RefreshBedRow(day.Bed);
        };
        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowGuestDetails(Guest guest)
    {
        _guestDetailsViewModelStore.GuestParameter = guest;
        _mainViewModelStore.Sub.NavigateTo(typeof(GuestDetailsViewModel));
    }

    [RelayCommand]
    private void ShowGuestForm(Occupancy occupancy)
    {
        _serviceScope = _createServiceScope();
        var form = (GuestInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestInputFormViewModel));
        form.EditEntity = occupancy!.Guest;
        form.IsClassInputEnabled = false;

        form.SaveCompleted = (guest, formWhileSaving) => RefreshBedRow(occupancy.Bed);

        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
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
        CurrentFormViewModel = null;
        _serviceScope?.Dispose();
        _mainViewModelStore.BlockView(false);
    }
}