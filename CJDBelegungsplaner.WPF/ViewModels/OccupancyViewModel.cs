using CJDBelegungsplaner.WPF.Stores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace CJDBelegungsplaner.WPF.ViewModels;

[INotifyPropertyChanged]
public partial class OccupancyViewModel : ViewModelBase
{
    public string Title { get; set; } = "Occupancy";

    private MainWindowViewModelStore _mainViewModelStore;

    public OccupancyViewModel(MainWindowViewModelStore mainViewModelStore)
	{
        _mainViewModelStore = mainViewModelStore;
    }

    [RelayCommand]
    public void NavigateTo(Type viewModelType)
    {
        _mainViewModelStore.Main.NavigateTo(viewModelType);
    }
}
//Testkommentar