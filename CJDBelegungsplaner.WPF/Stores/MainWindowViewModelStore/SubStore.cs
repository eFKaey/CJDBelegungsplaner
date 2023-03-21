using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CJDBelegungsplaner.WPF.Stores;

public partial class MainWindowViewModelStore
{
    public class SubStore
    {
        public event Action? CurrentViewModelChanged;

        private IViewModel? _currentViewModel = null;
        public IViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        public bool IsOpen => CurrentViewModel is not null;

        private readonly Func<IServiceScope> _createServiceScope;
        private IServiceScope _serviceScope;

        public SubStore(Func<IServiceScope> createServiceScope)
        {
            _createServiceScope = createServiceScope;
        }

        public void Close()
        {
            _serviceScope?.Dispose();
            CurrentViewModel = null;
        }

        public void NavigateTo(Type viewModelType)
        {
            _serviceScope?.Dispose();
            _serviceScope = _createServiceScope();
            CurrentViewModel = (IViewModel)_serviceScope.ServiceProvider.GetRequiredService(viewModelType);
        }
    }
}
