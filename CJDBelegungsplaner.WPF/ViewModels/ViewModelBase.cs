using System;
using CJDBelegungsplaner.WPF.ViewModels.Interfaces;

namespace CJDBelegungsplaner.WPF.ViewModels;

public abstract class ViewModelBase : IDisposable, IViewModel
{
    public virtual void Dispose()
    {
    }
}
