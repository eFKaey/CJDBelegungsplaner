using CJDBelegungsplaner.Domain.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CJDBelegungsplaner.WPF.Intermediate;

[INotifyPropertyChanged]
public partial class CheckableEntityObject<TModel>
    where TModel : EntityObject
{
    public TModel Entity { get; private set; }

    [ObservableProperty]
    private bool _isChecked;

    public CheckableEntityObject(TModel entity, bool isChecked)
    {
        Entity = entity;
        _isChecked = isChecked;
    }
}
