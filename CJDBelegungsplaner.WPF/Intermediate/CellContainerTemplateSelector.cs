using CJDBelegungsplaner.Domain.Models;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CJDBelegungsplaner.WPF.Intermediate;

public class CellContainerTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        // Nach meinen Tests komme ich zu dem Schluss, dass diese Methode jedes mal pro Zelle aufgerufen wird, sobald sich der DataContext ändert.
        //     Beim ersten Mal ist der DataContext null.
        //     Beim zweisten Mal wird dieser vom DataTable zur DataRowView.
        //     Beim dritten mal, weil ich den wieder auf den ReservationContainer zurücksetzte.

        if (container is null || item is null)
        {
            return null;
        }

        var element = container as FrameworkElement;

        if (item is DataRowView)
        {
            // Wenn DataRowView, dann zeigt der DataContext nicht auf den ReservationContainer, sondern eben auf die ganze Zeile.
            // Da ich bisher nicht herausgefunden habe, wie man im XAML dynamisch auf eine Zelle einer Zeile zugreift, wird hier der DataContext zurück auf den jeweiligen ReservationContainer gesetzt.
            var rowView = item as DataRowView;
            var cell = element.Parent as DataGridCell;
            int column = cell.Column.DisplayIndex;
            cell.DataContext = rowView.Row[column];
            return null;
        }

        if (item.GetType() == typeof(ReservationCellContainerOfTModelOfTReservation<Class, ClassReservation>))
        {
            return element.FindResource("ClassReservationContainerTemplate") as DataTemplate;
        }
        if (item.GetType() == typeof(ReservationCellContainerOfTModelOfTReservation<Guest, GuestReservation>))
        {
            return element.FindResource("GuestReservationContainerTemplate") as DataTemplate;
        }
        if (item.GetType() == typeof(ButtonCellContainer))
        {
            return element.FindResource("ButtonReservationContainerTemplate") as DataTemplate;
        }

        return null;
    }
}
