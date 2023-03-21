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
    /// Wird am Ende des Speicher/Update-Vorgangs ausgeführt.
    /// </summary>
    public Action<TModel>? SaveCompleted;

    public virtual TModel? EditEntity { get; set; }

    public bool IsNewEntity => EditEntity is null;

    /// <summary>
    /// Sollte die Validation-Logik beinhalten (ValidateProperty(PROPERTY)...).
    /// Wenn null wird ValidateAllProperties() ausgeführt.
    /// </summary>
    public Action? ExecuteValidation;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SaveCompleted is null)
        {
            throw new NullReferenceException(nameof(SaveCompleted));
        }

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

        SaveCompleted(entity);

        Close();
    }

    public abstract Task<(bool, TModel)> CreateAsync();

    public abstract Task<(bool, TModel)> UpdateAsync();

    public override void Dispose()
    {
        EditEntity = null;
        SaveCompleted = null;
        base.Dispose();
    }
}
