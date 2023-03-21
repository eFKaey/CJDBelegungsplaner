using CJDBelegungsplaner.WPF.ViewModels;
using MaterialDesignThemes.Wpf;
using System.Windows;

namespace CJDBelegungsplaner.WPF;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        /// Aus MainWindow.xaml hier her verlegt:
        ///     <Window.DataContext>
        ///         <viewModels:MainViewModel/>
        ///     </Window.DataContext>
        DataContext = viewModel;
    }
}
