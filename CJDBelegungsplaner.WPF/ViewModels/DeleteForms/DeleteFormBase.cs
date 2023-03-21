using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.DeleteForms;

[INotifyPropertyChanged]
public abstract partial class DeleteFormBase : IDisposable, IViewModel
{
    /// <summary>
    /// Sollte die Logik für das Schließen beinhalten.
    /// </summary>
    public Action? ExecuteClose;

    /// <summary>
    /// Wird am Ende des Speicher/Update-Vorgangs ausgeführt.
    /// </summary>
    public Action? DeleteCompleted;

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

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (DeleteCompleted is null)
        {
            throw new NullReferenceException(nameof(DeleteCompleted));
        }

        IsEnabled = false;

        bool isFailure = await ExecuteDeleteAsync();

        IsEnabled = true;

        if (isFailure)
        {
            return;
        }

        DeleteCompleted();

        Close();
    }

    public abstract Task<bool> ExecuteDeleteAsync();

    [RelayCommand]
    protected void Close()
    {
        if (ExecuteClose is null)
        {
            throw new NullReferenceException(nameof(ExecuteClose));
        }

        ExecuteClose();
    }

    public virtual void Dispose()
    {
        ExecuteClose = null;
        DeleteCompleted = null;
    }
}
