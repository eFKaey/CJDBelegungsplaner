using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Utility.ExtensionMethods;
                  
public static class ObservableCollectionExtension
{
    public static void ReplaceItem<T>(this ObservableCollection<T> items, Func<T, bool> predicate, T newItem, object? dummy = null)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (predicate(items[i]))
            {
                // Workaround
                //
                // Problem:
                // Der ObservableCollection das selbe Objekt an der selben Stelle wieder zuweisen
                // triggert nicht INotifyChanged in der Collection.
                // Mögliche Begründung:
                // Es wird ein Referenzvergleich durchgeführt und abbgebrochen wenn identisch.
                // Benutzter Workaround:
                // Als Parameter kann ein Dummy-Objekt mitgegeben werden, welches kurzfristig den 
                // Platz einnimmt.

                if (dummy is not null) 
                {
                    if (dummy.GetType() != typeof(T)) {
                        throw new ArgumentException("Das gegebene Dummy-Objekt passt nicht zu dem Item-Type!"); }
                    items[i] = (T)Activator.CreateInstance(typeof(T)); 
                }
                
                items[i] = newItem;
                break;
            }
        }
    }
}
