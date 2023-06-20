using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.WPF.Stores;
using System.Collections.Generic;
using System.Windows.Data;
using System;

namespace CJDBelegungsplaner.WPF.ViewModels.Filter;

public class LogEntryListFilter : FilterBase
{
    private LogEntryListViewModelStore _logEntryListViewModelStore;

    public List<string> UserNames { get; private set; }

    #region Binding Properties

    public string? Name
    {
        get => _logEntryListViewModelStore.FilterName;
        set
        {
            _logEntryListViewModelStore.FilterName = value;
            OnPropertyChangedAndDelayRefresh(nameof(Name));
        }
    }
    public string? Action
    {
        get => _logEntryListViewModelStore.FilterAction;
        set
        {
            _logEntryListViewModelStore.FilterAction = value;
            OnPropertyChangedAndDelayRefresh(nameof(Action));
        }
    }

    public string? DateFrom
    {
        get => _logEntryListViewModelStore.FilterDateFrom;
        set
        {
            _logEntryListViewModelStore.FilterDateFrom = value;
            OnPropertyChangedAndDelayRefresh(nameof(DateFrom));
        }
    }
    public string? DateTo
    {
        get => _logEntryListViewModelStore.FilterDateTo;
        set
        {
            _logEntryListViewModelStore.FilterDateTo = value;
            OnPropertyChangedAndDelayRefresh(nameof(DateTo));
        }
    }

    #endregion

    public LogEntryListFilter(LogEntryListViewModelStore logEntryListViewModelStore, Action refresh, List<string> userNames)
        : base(refresh)
    {
        _logEntryListViewModelStore = logEntryListViewModelStore;

        UserNames = userNames;

        InitializeIsActive(); // Muss immer am Ende des Konstruktors sein!
    }

    public void OnFilter(object sender, FilterEventArgs e)
    {
        var row = e.Item as LogEntry;

        if (row is null)
        {
            return;
        }

        DateTime dateFrom;
        DateTime dateTo;
        bool isName = true;
        bool isAction = true;
        bool isDateFrom = true;
        bool isDateTo = true;

        if (!string.IsNullOrEmpty(Name))
        {
            if (row.User is null)
            {
                isName = false;
            }
            else
            {
                isName = row.User.Name.Contains(Name, StringComparison.OrdinalIgnoreCase);
            }
        }
        if (!string.IsNullOrEmpty(Action))
        {
            isName = row.Action.Contains(Action, StringComparison.OrdinalIgnoreCase);
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

        e.Accepted = isName && isAction && isDateFrom && isDateTo;
    }

}
