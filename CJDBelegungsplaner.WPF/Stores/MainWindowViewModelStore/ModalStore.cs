using CJDBelegungsplaner.WPF.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CJDBelegungsplaner.WPF.Stores;

public partial class MainWindowViewModelStore
{
    public class ModalStore
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

        public ModalStore(Func<IServiceScope> createServiceScope)
        {
            _createServiceScope = createServiceScope;
        }

        public void Close()
        {
            _serviceScope?.Dispose();
            CurrentViewModel = null;
        }

        public void NavigateTo(Type viewModelType, Action<IViewModel> action = null)
        {
            _serviceScope?.Dispose();
            _serviceScope = _createServiceScope();
            var viewModel = (IViewModel)_serviceScope.ServiceProvider.GetRequiredService(viewModelType);
            if (action is not null)
            {
                action(viewModel);
            }
            CurrentViewModel = viewModel;
        }
    }
}
