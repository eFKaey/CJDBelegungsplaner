using System.Windows.Data;
using System;
using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;

namespace CJDBelegungsplaner.WPF.ViewModels.Filter;

public class ClassListFilter : FilterBase
{
    private ClassListViewModelStore _classListViewModelStore;

    #region Binding Properties

    public string? Name
    {
        get => _classListViewModelStore.FilterName;
        set
        {
            _classListViewModelStore.FilterName = value;
            OnPropertyChangedAndDelayRefresh(nameof(Name));
        }
    }

    public string? DateFrom
    {
        get => _classListViewModelStore.FilterDateFrom;
        set
        {
            _classListViewModelStore.FilterDateFrom = value;
            OnPropertyChangedAndDelayRefresh(nameof(DateFrom));
        }
    }
    public string? DateTo
    {
        get => _classListViewModelStore.FilterDateTo;
        set
        {
            _classListViewModelStore.FilterDateTo = value;
            OnPropertyChangedAndDelayRefresh(nameof(DateTo));
        }
    }

    #endregion

    public ClassListFilter(ClassListViewModelStore logEntryListViewModelStore, Action refresh)
        : base(refresh)
    {
        _classListViewModelStore = logEntryListViewModelStore;

        InitializeIsActive(); // Muss immer am Ende des Konstruktors sein!
    }

    public void OnFilter(object sender, FilterEventArgs e)
    {
        var row = e.Item as Class;

        if (row is null)
        {
            return;
        }

        DateTime dateFrom;
        DateTime dateTo;
        bool isName = true;
        bool isDateFrom = true;
        bool isDateTo = true;

        if (!string.IsNullOrEmpty(Name))
        {
            isName = row.Name.Contains(Name, StringComparison.OrdinalIgnoreCase);
        }
        if (!string.IsNullOrEmpty(DateFrom)
            && DateTime.TryParse(DateFrom, out dateFrom))
        {
            isDateFrom = dateFrom <= row.Created;
        }
        if (!string.IsNullOrEmpty(DateTo)
            && DateTime.TryParse(DateTo, out dateTo))
        {
            isDateTo = row.Created <= dateTo;
        }

        e.Accepted = isName && isDateFrom && isDateTo;
    }

}
