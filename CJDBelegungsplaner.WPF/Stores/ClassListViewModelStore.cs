namespace CJDBelegungsplaner.WPF.Stores;

public class ClassListViewModelStore
{
    public string? FilterName { get; set; } = string.Empty;
    public string? FilterDateFrom { get; set; } = string.Empty;
    public string? FilterDateTo { get; set; } = string.Empty;

    public void ClearFilter()
    {
        FilterName = string.Empty;
        FilterDateFrom = string.Empty;
        FilterDateTo = string.Empty;
    }
}
