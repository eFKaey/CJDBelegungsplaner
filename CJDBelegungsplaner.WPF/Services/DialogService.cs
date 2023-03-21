using CJDBelegungsplaner.WPF.Services.Interfaces;
using CJDBelegungsplaner.WPF.Stores;
using CJDBelegungsplaner.WPF.ViewModels;
using Microsoft.Win32;

namespace CJDBelegungsplaner.WPF.Services;

public class DialogService : IDialogService
{
    private readonly MainWindowViewModelStore _mainViewModelStore;
    private readonly MessageDialogViewModelStore _messageDialogViewModelStore;

    public DialogService(MainWindowViewModelStore mainViewModelStore, MessageDialogViewModelStore messageDialogViewModelStore)
    {
        _mainViewModelStore = mainViewModelStore;
        _messageDialogViewModelStore = messageDialogViewModelStore;
    }

    public bool OpenFileDialog(bool checkFileExists, string Filter, out string FileName)
    {
        FileName = string.Empty;

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Multiselect = false,
            Filter = Filter, //= "All Image Files | *.jpg;*.png | All files | *.*",
            CheckFileExists = checkFileExists
        };

        bool result = (bool)openFileDialog.ShowDialog()!;

        if (result) {
            FileName = openFileDialog.FileName; }

        return result;
    }

    public MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon)
    {
        return (MessageBoxResult)System.Windows.MessageBox.Show(
            message, 
            caption,
            (System.Windows.MessageBoxButton)buttons,
            (System.Windows.MessageBoxImage)icon);
    }

    public void ShowMessageDialog(string message, string title, MessageBoxImage icon = MessageBoxImage.None)
    {
        _messageDialogViewModelStore.Message = message;
        _messageDialogViewModelStore.Title = title;
        _mainViewModelStore.Modal.NavigateTo(typeof(MessageDialogViewModel));
    }
}