using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class MessageDialogViewModel : ViewModelBase
{
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly MessageDialogViewModelStore _messageDialogViewModelStore;

    public string? Message => _messageDialogViewModelStore.Message;
    public string? Title => _messageDialogViewModelStore.Title;

    public MessageDialogViewModel(MainWindowViewModelStore mainViewModelStore, MessageDialogViewModelStore messageDialogViewModelStore)
    {
        _mainViewModelStore = mainViewModelStore;
        _messageDialogViewModelStore = messageDialogViewModelStore;
    }

    [RelayCommand]
    private void Close()
    {
        _mainViewModelStore.Modal.Close();
    }
}
