namespace CJDBelegungsplaner.WPF.Stores;

public class LogEntryListViewModelStore
{
    public string? FilterName { get; set; } = string.Empty;
    public string? FilterAction { get; set; } = string.Empty;
    public string? FilterDateFrom { get; set; } = string.Empty;
    public string? FilterDateTo { get; set; } = string.Empty;

    public void ClearFilter()
    {
        FilterName = string.Empty;
        FilterAction = string.Empty;
        FilterDateFrom = string.Empty;
        FilterDateTo = string.Empty;
    }
}
