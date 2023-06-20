using System;
using CJDBelegungsplaner.Domain.Models;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CJDBelegungsplaner.WPF.ViewModels.InputForms;

/// <summary>
/// Generische Basisklasse für Input-Forms mit Eingabefelder die Validiert werden sollen.
/// </summary>
public abstract partial class InputFormBase<TModel> : InputFormBase
    where TModel : EntityObject
{
    /// <summary>
    /// Ein Delegate, der am Ende des Speicher/Update-Vorgangs ausgeführt wird.
    /// </summary>
    public Action<TModel, InputFormBase<TModel>>? SaveCompleted;

    /// <summary>
    /// Enthält die zu bearbeitende Entität.
    /// </summary>
    public virtual TModel? EditEntity { get; set; }

    public bool IsNewEntity => EditEntity is null;

    /// <summary>
    /// Delegate, der die Validation-Logik beinhalten sollte (ValidateProperty(PROPERTY)...).
    /// Wenn null wird ValidateAllProperties() ausgeführt.
    /// </summary>
    public Action? ExecuteValidation;

    /// <summary>
    /// Metthode, die den Speichervorgang ausführt. Wird durch das drücken des Speicherbuttons
    /// aufgerufen.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (ExecuteValidation is null)
        {
            ValidateAllProperties();
        }
        else
        {
            ExecuteValidation();
        }

        if (HasErrors)
        {
            return;
        }

        bool isFailure;
        TModel entity;

        IsFormEnabled = false;

        if (IsNewEntity)
        {
            (isFailure, entity) = await CreateAsync();
        }
        else
        {
            (isFailure, entity) = await UpdateAsync();
        }

        IsFormEnabled = true;

        if (isFailure)
        {
            return;
        }

        if (SaveCompleted is not null)
        {
            SaveCompleted(entity, this);
        }

        Close();
    }

    /// <summary>
    /// Abstrakte Methode zum Erstellen einer neuen Entität.
    /// </summary>
    public abstract Task<(bool, TModel)> CreateAsync();

    /// <summary>
    /// Abstrakte Methode zum Bearbeiten einer vorhandenen Entität.
    /// </summary>
    public abstract Task<(bool, TModel)> UpdateAsync();

    public override void Dispose()
    {
        EditEntity = null;
        SaveCompleted = null;
        base.Dispose();
    }
}
