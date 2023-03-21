using CJDBelegungsplaner.WPF.Intermediate;
using System.Data;
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

        e.Column = new DataGridTemplateColumn
        {
            /// das usprüngliche HeaderTemplate verwenden
            HeaderTemplate = e.Column.HeaderTemplate,
            /// Caption soll als header angezeigt werden
            Header = (dataGrid.ItemsSource as DataView).Table.Columns[e.PropertyName].Caption,
            /// einfaches Template in Resources
            //CellTemplate = dataGrid.Resources["ImgCell"] as DataTemplate,
            /// TemplateSelector für verschiedene Templates
            CellTemplateSelector = new CellContainerTemplateSelector(),
        };
    }
}