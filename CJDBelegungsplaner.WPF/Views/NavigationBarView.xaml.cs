using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CJDBelegungsplaner.WPF.Views
{
    /// <summary>
    /// Interaction logic for NavigationBarView.xaml
    /// </summary>
    public partial class NavigationBarView : UserControl
    {
        [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
        public static extern bool ShouldSystemUseDarkMode();

        public NavigationBarView()
        {
            InitializeComponent();

            if (ShouldSystemUseDarkMode())
            {
                DarkModeToggleButton.IsChecked = true;
                ModifyTheme(true);
            }
        }

        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e)
            => ModifyTheme(DarkModeToggleButton.IsChecked == true);


        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }
    }
}
