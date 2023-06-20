using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Intermediate;
using CJDBelegungsplaner.WPF.ViewModels;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CJDBelegungsplaner.WPF.Views;

/// <summary>
/// Interaction logic for ReservationView.xaml
/// </summary>
public partial class ReservationView : UserControl
{
    public ReservationView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Event-Methode, welche den DataGrid-Headern die Caption-Property der DataTable zuweist. Standardmaßig ist es die Name-Property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReservationDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        //e.Column.Header = ((sender as DataGrid).ItemsSource as DataView).Table.Columns[e.PropertyName].Caption;

        var dataGrid = sender as DataGrid;

        var column = (dataGrid.ItemsSource as DataView).Table.Columns[e.PropertyName] as DataColumnWeek;

        e.Column = new DataGridTemplateColumn
        {
            /// das usprüngliche HeaderTemplate verwenden
            //HeaderTemplate = e.Column.HeaderTemplate,
            HeaderTemplate = dataGrid.Resources["ReservationDataGridColumnHeaderTemplate"] as DataTemplate,
            /// die DataColumnWeek als DataContext für den Header; kann so als Binding im Headertemplate verwendet werden
            Header = column, 
            /// einfaches Template in Resources
            //CellTemplate = dataGrid.Resources["ImgCell"] as DataTemplate,
            /// TemplateSelector für verschiedene Templates
            CellTemplateSelector = new CellContainerTemplateSelector(),
        };
    }

    /// <summary>
    /// Ein Workaround, da sehr eigenartigerweise der Command-Parameter (XAML) beim MenueItem nicht funktioniert,
    /// wenn ein ItemsSource verwendet wird. Zumindest erscheint es so, was dir wirkliche Ursache ist, weß ik och nich.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ShowGuestDetailsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var guest = (e.OriginalSource as MenuItem)!.Header as Guest;
        (DataContext as ReservationViewModel)!.ShowGuestDetailsCommand.Execute(guest);
    }
}