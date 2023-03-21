namespace CJDBelegungsplaner.WPF.Stores;

public class GuestListViewModelStore
{
    public string? FilterFirstName { get; set; } = string.Empty;
    public string? FilterLastName { get; set; } = string.Empty;
    public string? FilterBirthdateFrom { get; set; } = string.Empty;
    public string? FilterBirthdateTo { get; set; } = string.Empty;
    public string? FilterClassName { get; set; } = string.Empty;
    public string? FilterCompanyName { get; set; } = string.Empty;

    public void ClearFilter()
    {
        FilterFirstName = string.Empty;
        FilterLastName = string.Empty;
        FilterBirthdateFrom = string.Empty;
        FilterBirthdateTo = string.Empty;
        FilterClassName = string.Empty;
        FilterCompanyName = string.Empty;
    }
}
