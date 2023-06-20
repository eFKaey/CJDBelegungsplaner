using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System;
using CommunityToolkit.Mvvm.Input;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

/// <summary>
/// Basisklasse für Input-Forms mit Eingabefelder die Validiert werden sollen.
/// </summary>
public abstract partial class InputFormBase : ObservableValidator, IDisposable, IViewModel
{

    /// <summary>
    /// Sollte die Logik für das Schließen beinhalten.
    /// </summary>
    public Action? ExecuteClose;

    private bool _isFormEnabled = true;
    public bool IsFormEnabled 
    { 
        get => _isFormEnabled;
        set 
        {
            _isFormEnabled = value;
            OnPropertyChanged(nameof(IsFormEnabled));
            OnPropertyChanged(nameof(IsFormDisabled));
        } 
    }

    public bool IsFormDisabled
    { 
        get => !IsFormEnabled;
        set 
        {
            _isFormEnabled = !value;
            OnPropertyChanged(nameof(IsFormDisabled));
            OnPropertyChanged(nameof(IsFormEnabled));
        } 
    }

    // TODO: delete, weil der ObservableValidator das übernimmt
    protected string GetErrorMessage(string propertyName)
    {
        return string.Join(
            Environment.NewLine,
            GetErrors(propertyName).Select(e => e.ErrorMessage));
    }

    public new void ValidateAllProperties()
        => base.ValidateAllProperties();

    public new void ValidateProperty(object? value, string? propertyName = null)
        => base.ValidateProperty(value, propertyName!);

    [RelayCommand]
    protected void Close()
    {
        if (ExecuteClose is null)
        {
            return;
        }

        ExecuteClose();
    }

    public virtual void Dispose()
    {
        ExecuteClose = null;
    }
}
