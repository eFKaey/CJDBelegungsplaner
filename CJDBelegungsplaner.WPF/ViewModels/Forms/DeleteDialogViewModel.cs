using CommunityToolkit.Mvvm.ComponentModel;

namespace CJDBelegungsplaner.WPF.ViewModels.Forms;

[INotifyPropertyChanged]
public partial class DeleteDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _deleteMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDisabled))]
    private bool _isEnabled = true;

    public bool IsDisabled
    {
        get => !IsEnabled;
        set
        {
            _isEnabled = !value;
            OnPropertyChanged(nameof(IsDisabled));
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
}
