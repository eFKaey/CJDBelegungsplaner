using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CJDBelegungsplaner.WPF.Stores;

public partial class MainWindowViewModelStore
{
    public class MainStore
    {
        public event Action? CurrentViewModelChanged;
        public IViewModel? CurrentViewModel { get; set; }

        private readonly Func<IServiceScope> _createServiceScope;
        private IServiceScope _serviceScope;

        public MainStore(Func<IServiceScope> createServiceScope)
        {
            _createServiceScope = createServiceScope;
        }

        public void NavigateTo(Type viewModelType)
        {
            _serviceScope?.Dispose();
            _serviceScope = _createServiceScope();
            CurrentViewModel = (IViewModel)_serviceScope.ServiceProvider.GetRequiredService(viewModelType);
            CurrentViewModelChanged?.Invoke();
        }
    }
}
