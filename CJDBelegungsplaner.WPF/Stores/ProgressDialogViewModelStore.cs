using System;

namespace CJDBelegungsplaner.WPF.Stores;

public class ProgressDialogViewModelStore
{
    public event Action? DatabaseConnectionCheck;

    public void CheckDatabaseConnection()
    {
        DatabaseConnectionCheck?.Invoke();
    }
}
