using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CJDBelegungsplaner.WPF.ViewModels.Filter;

/// <summary>
/// <para>
/// Basisklasse für Filter von z.B. DataGrid-ListViews.
/// </para>
/// <para>
/// Beinhaltet:<br/>
/// - Einen Action Delegat für den ListView-Event, der asugelöst wird, wenn eine Eingabe getätigt wurde.<br/>
/// - Einen Timer, der die Benutzereingabe abwartet und dann den Action Delegat ausführt.<br/>
/// - ObservableObject für Bindings.<br/>
/// </para>
/// </summary>
public abstract class FilterBase : ObservableObject, IDisposable
{
    public Action Refresh { get; set; }

    private static DispatcherTimer Timer = new DispatcherTimer();

    Dictionary<string, bool> FiltersState = new Dictionary<string, bool>();

    private bool _isActive = false;
    public bool IsActive
    {
        get => _isActive;
        set 
        {
            _isActive = value;
            OnPropertyChanged(nameof(IsActive));
        }
    }

    protected FilterBase(Action refresh)
    {
        Refresh += refresh;
        Timer.Tick += OnTimedRefreshEvent!;
    }

    private void OnTimedRefreshEvent(object sender, EventArgs e)
    {
        Timer.Stop();
        Refresh();
    }

    /// <summary>
    /// Startet den Timer, ruft OnPropertyChanged auf und setzt IsActive.
    /// </summary>
    /// <param name="propertyName"></param>
    /// <exception cref="Exception"></exception>
    protected void OnPropertyChangedAndDelayRefresh(string propertyName)
    {
        Timer.Interval = TimeSpan.FromMilliseconds(500);
        Timer.Start();

        OnPropertyChanged(propertyName);

        object? value = GetType().GetProperty(propertyName)?.GetValue(this, null);

        if (value is not null && value.GetType() != typeof(string))
        {
            // TODO: Custom Exception
            throw new Exception("Momentan ist es nur mit Type string gedacht.");
        }

        FiltersState[propertyName] = !string.IsNullOrEmpty(value as string);
        IsActive = FiltersState.ContainsValue(true);
    }

    /// <summary>
    /// Überprüft alle string-Properties der Kind Klasse auf IsNullOrEmpty und setzt entsprechend IsActive.
    /// Dabei wird angenommen, dass alle string-Properties zum Filter gehören (Eingabefelder). Alle anderen Typen und alle Properties der Elternklasse (FilterBase) werden ignoriert.
    /// Sollte ein Property der Kindklasse vom Type string sein und nicht zum Filter gehören wird es wahrscheinlich zu einem semantischen Fehler führen!
    /// </summary>
    protected void InitializeIsActive()
    {
        System.Reflection.PropertyInfo[] props = GetType().GetProperties();
        for (int i = 0; i < props.Length; i++)
        {
            if (typeof(FilterBase).GetProperty(props[i].Name) is null
                && props[i].PropertyType == typeof(string))
            {
                var value = props[i].GetValue(this, null) as string;
                FiltersState[props[i].Name] = !string.IsNullOrEmpty(value);
            }
        }
        IsActive = FiltersState.ContainsValue(true);
    }

    public void Dispose()
    {
        Timer.Tick -= OnTimedRefreshEvent!;
    }
}
