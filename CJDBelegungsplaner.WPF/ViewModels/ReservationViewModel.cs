using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using CJDBelegungsplaner.Domain.Models.Interfaces;
using CJDBelegungsplaner.WPF.Intermediate;
using System.Data;
using CJDBelegungsplaner.WPF.ViewModels.InputForms;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Utility.ExtensionMethods;
using System.ComponentModel;
using System.Windows.Data;
using CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class ReservationViewModel : ViewModelBase
{
    #region Properties/Fields

    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly Func<IServiceScope> _createServiceScope;
    private readonly DataStore _dataStore;

    private IServiceScope _serviceScope;

    [ObservableProperty]
    private string _startDate;

    public int[] ColumnAmountSource { get; set; } = new int[] { 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 27, 33, 37, 43, 47, 52 };

    [ObservableProperty]
    private int _columnAmount;

    [ObservableProperty]
    private ObservableCollection<Guest>? _guests;

    [ObservableProperty]
    private ObservableCollection<Class>? _classes;

    [ObservableProperty]
    private IViewModel? _currentFormViewModel;

    [ObservableProperty]
    private bool _isDialogOpen;

    private ReservationCellContainer? _deleteContainer;

    [ObservableProperty]
    private ObservableCollection<Week>? _weeks = new ObservableCollection<Week>();

    [ObservableProperty]
    private DataTable _dataTable = new DataTable("Weeks");

    private int _bedCount = 12;

    #endregion

    #region Konstruktor

    public ReservationViewModel(
        MainWindowViewModelStore mainViewModelStore,
        Func<IServiceScope> createServiceScope,
        DataStore dataStore)
    {
        _mainViewModelStore = mainViewModelStore;
        _createServiceScope = createServiceScope;
        _dataStore = dataStore;

        StartDate = "27.02.2023";//DateTime.Now.ToString("d");
        ColumnAmount = 15;

        _mainViewModelStore.Modal.NavigateTo(typeof(ProgressDialogViewModel));

        LoadDataCommand.Execute(null);
    }

    #endregion

    [RelayCommand]
    //private void LoadDataAsync()
    private async Task LoadDataAsync()
    {
        Classes = await _dataStore.GetClassesAsync(true);

        //Classes = _dataStore.GetClasses(true);

        if (Classes is null)
        {
            return;
        }

        Guests = await _dataStore.GetGuestsAsync(true);

        //Guests = _dataStore.GetGuests(true);

        if (Guests is null)
        {
            return;
        }

        CreateColumns();
        FillColumns();
        AddButtons(0, DataTable.Columns.Count);

        DataTable uglyWorkaround = DataTable;
        DataTable = null;
        DataTable = uglyWorkaround;

        //System.Windows.MessageBox.Show("sdasd");

        //OnPropertyChanged(nameof(DataTable));

        //CollectionViewSource.GetDefaultView(DataTable).Refresh();

        _mainViewModelStore.Modal.Close();
    }

    // ---------------------------------------------

    [RelayCommand]
    private void ShowReservationFormForCreating(ButtonCellContainer container)
    {
        ShowReservationForm((form) =>
        {
            form.SaveClassReservationCompleted = (reservation) =>
            {
                AppendReservation(reservation.Class, reservation);
                CollectionViewSource.GetDefaultView(DataTable).Refresh();
            };
            form.SaveGuestReservationCompleted = (reservation) =>
            {
                AppendReservation(reservation.Guest, reservation);
                CollectionViewSource.GetDefaultView(DataTable).Refresh();
            };

            form.Begin = container.DataColumnWeek.StartDate.ToString("d");
            form.End = container.DataColumnWeek.StartDate.AddDays(5).ToString("d");

            form.ShowClassReservationFormCommand.Execute(null);
        });
    }

    [RelayCommand]
    private void ShowReservationFormForUpdatingClassReservation(ReservationCellContainerOfTModelOfTReservation<Class, ClassReservation> container)
    {
        ShowReservationForm((form) =>
        {
            form.EditReservation = container.Reservation;

            form.SaveClassReservationCompleted = (reservation) =>
            {
                RemoveContainerFormDataTable(container);
                AppendReservation(reservation.Class, reservation);
                CollectionViewSource.GetDefaultView(DataTable).Refresh();
            };

            form.Begin = container.Reservation.Begin.ToString("d");
            form.End = container.Reservation.End.ToString("d");

            form.IsClassFormButtonEnabled = false;
            form.IsGuestFormButtonEnabled = false;

            form.ShowClassReservationFormCommand.Execute(null);
        });
    }

    [RelayCommand]
    private void ShowReservationFormForUpdatingGuestReservation(ReservationCellContainerOfTModelOfTReservation<Guest, GuestReservation> container)
    {
        ShowReservationForm((form) =>
        {
            form.EditReservation = container.Reservation;

            form.SaveGuestReservationCompleted = (reservation) =>
            {
                RemoveContainerFormDataTable(container);
                AppendReservation(reservation.Guest, reservation);
                CollectionViewSource.GetDefaultView(DataTable).Refresh();
            };

            form.Begin = container.Reservation.Begin.ToString("d");
            form.End = container.Reservation.End.ToString("d");

            form.IsClassFormButtonEnabled = false;
            form.IsGuestFormButtonEnabled = false;
            form.IsGuestFormButtonChecked = true;

            form.ShowGuestReservationFormCommand.Execute(null);
        });
    }

    [RelayCommand]
    private void ShowDeleteClassReservationMessage(ReservationCellContainerOfTModelOfTReservation<Class, ClassReservation> container)
    {
        if (container is null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        _serviceScope = _createServiceScope();
        var form = (ClassReservationDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ClassReservationDeleteFormViewModel));
        form.DeleteMessage = "Möchten Sie diese Klassen-Reservierung wirklich löschen?";
        form.DeleteEntity = container.Reservation;
        form.DeleteCompleted = () =>
        {
            RemoveContainerFormDataTable(container);
            CollectionViewSource.GetDefaultView(DataTable).Refresh();
        };
        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    [RelayCommand]
    private void ShowDeleteGuestReservationMessage(ReservationCellContainerOfTModelOfTReservation<Guest, GuestReservation> container)
    {
        if (container is null)
        {
            throw new ArgumentNullException(nameof(container));
        }

        _serviceScope = _createServiceScope();
        var form = (GuestReservationDeleteFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(GuestReservationDeleteFormViewModel));
        form.DeleteMessage = "Möchten Sie diese Gast-Reservierung wirklich löschen?";
        form.DeleteEntity = container.Reservation;
        form.DeleteCompleted = () =>
        {
            RemoveContainerFormDataTable(container);
            CollectionViewSource.GetDefaultView(DataTable).Refresh();
        };
        form.ExecuteClose = CloseDialog;

        CurrentFormViewModel = form;

        OpenDialog();
    }

    // ---------------------------------------------

    private void ShowReservationForm(Action<ReservationInputFormViewModel> action)
    {
        _serviceScope = _createServiceScope();

        var form = (ReservationInputFormViewModel)_serviceScope.ServiceProvider.GetRequiredService(typeof(ReservationInputFormViewModel));

        form.ExecuteClose = CloseDialog;

        action(form);

        CurrentFormViewModel = form;

        OpenDialog();
    }

    private void AppendReservation<TModel, TReservation>(TModel entity, TReservation reservation)
            where TModel : EntityObject, IModelWithReservation<TReservation>
            where TReservation : Reservation
    {
        DateTime startDate = DateTime.Parse(StartDate);
        DateTime endDate = startDate.AddDays(7 * ColumnAmount);
        (int colStart, int colEnd) = GetColumns(startDate, endDate, reservation);
        int row = FindFreeRow(colStart, colEnd);
        CreateRowsUpTo(row + 1);
        AddContainer(entity, reservation, colStart, colEnd, row);
        AddButtons(colStart, colEnd + 1);
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
        _serviceScope?.Dispose();
        _mainViewModelStore.BlockView(false);
    }

    private void RemoveContainerFormDataTable(ReservationCellContainer deleteContainer)
    {
        for (int i = deleteContainer.RelatedContainers.Count - 1; i > -1; i--)
        {
            ReservationCellContainer? container = deleteContainer.RelatedContainers[i];
            DataTable.Rows[container.Row][container.Column] = DBNull.Value;
            deleteContainer.RelatedContainers.Remove(container);
        }
    }

    // ---------------------------------------------

    private void CreateColumns()
    {
        // TODO: _bedCount dynamisch machen.
        // Der user hat eine Einstellungsseite, auf der er angeben kann, ab welchem datum sich die Belegungszahl ändert.
        // Dafür wird natülcih eine Table benötigt.
        // Hier wird auf die Info zugegriffen.

        DateTime date = DateTime.Parse(StartDate);
        int amount = ColumnAmount - Weeks!.Count;
        DataColumn column;

        for (int number = 0; number < amount; number++)
        {
            column = new DataColumnWeek(
                number, 
                typeof(CellContainer), 
                date, 
                _bedCount);

            DataTable.Columns.Add(column);

            date = date.AddDays(7);
        }
    }

    private void FillColumns()
    {
        for (int i = 0; i < Classes?.Count; i++)
        {
            EvaluateReservations<Class, ClassReservation>(Classes[i]);
        }

        for (int i = 0; i < Guests?.Count; i++)
        {
            EvaluateReservations<Guest, GuestReservation>(Guests[i]);
        }
    }

    private void EvaluateReservations<TModel, TReservation>(TModel entity)
            where TModel : EntityObject, IModelWithReservation<TReservation>
            where TReservation : Reservation
    {
        List<TReservation> reservations = entity.Reservations.ToList();

        DateTime startDate = DateTime.Parse(StartDate);
        DateTime endDate = startDate.AddDays(7 * ColumnAmount);

        int colStart;
        int colEnd;
        int row;

        for (int r = 0; r < reservations.Count; r++)
        {
            (colStart, colEnd) = GetColumns(startDate, endDate, reservations[r]);

            if (colStart == -1)
            {
                continue;
            }

            row = FindFreeRow(colStart, colEnd);
            CreateRowsUpTo(row + 1);
            AddContainer(entity, reservations[r], colStart, colEnd, row);
        }
    }

    private (int colStart, int colEnd) GetColumns<TReservation>(DateTime startDate, DateTime endDate, TReservation reservation)
        where TReservation : Reservation
    {
        int colStart = -1;
        int colEnd = -1;

        if (startDate <= reservation.Begin && reservation.Begin <= endDate)
        {
            colStart = reservation.Begin.GetWeekISO8601() - startDate.GetWeekISO8601();

            if (reservation.End <= endDate)
            {
                colEnd = colStart + reservation.End.GetWeekISO8601() - reservation.Begin.GetWeekISO8601();
            }
            else
            {
                colEnd = colStart + endDate.GetWeekISO8601() - startDate.GetWeekISO8601();
            }
        }

        return (colStart, colEnd);
    }

    private int FindFreeRow(int colStart, int colEnd)
    {
        int row;
        object? cellValue;
        Type type;
        Type buttonType = typeof(ButtonCellContainer);
        Type dbNullType = typeof(DBNull);
        bool isFree;

        for (row = 0; row < DataTable.Rows.Count; row++)
        {
            isFree = true;

            for (int column = colStart; column <= colEnd; column++)
            {
                cellValue = DataTable.Rows[row][column];
                type = cellValue.GetType();

                if (type != dbNullType && type != buttonType)
                {
                    isFree = false;
                    break;
                }
            }

            if (isFree)
            {
                break;
            }
        }

        return row;
    }

    private void CreateRowsUpTo(int amount)
    {
        int newRows = amount - DataTable.Rows.Count;

        for (int i = 0; i < newRows; i++)
        {
            DataTable.Rows.Add(DataTable.NewRow());
        }
    }

    private void AddContainer<TModel, TReservation>(TModel entity, TReservation reservation, int colStart, int colEnd, int row)
            where TModel : EntityObject, IModelWithReservation<TReservation>
            where TReservation : Reservation
    {
        DataColumnWeek dataColumn;
        ReservationCellContainer? container = null;

        for (int column = colStart; column <= colEnd; column++)
        {
            dataColumn = DataTable.Columns[column] as DataColumnWeek;

            container = new ReservationCellContainerOfTModelOfTReservation<TModel, TReservation>(dataColumn, row, column, entity, reservation, container);

            DataTable.Rows[row][column] = container;
            dataColumn.BedCount--;

            if (dataColumn.ButtonRow > row)
            {
                continue;
            }

            if (DataTable.Rows[dataColumn.ButtonRow][column].GetType() == typeof(ButtonCellContainer))
            {
                DataTable.Rows[dataColumn.ButtonRow][column] = DBNull.Value;
            }

            dataColumn.ButtonRow = row + 1;
        }
    }

    private void AddButtons(int startCol, int colCount)
    {
        DataColumnWeek dataColumnWeek;

        for (int column = startCol; column < colCount; column++)
        {
            dataColumnWeek = DataTable.Columns[column] as DataColumnWeek;

            CreateRowsUpTo(dataColumnWeek.ButtonRow + 1);

            if (DataTable.Rows[dataColumnWeek.ButtonRow][column].GetType() != typeof(ButtonCellContainer))
            {
                DataTable.Rows[dataColumnWeek.ButtonRow][column] = dataColumnWeek.ButtonCellContainer;
            }
        }
    }
}