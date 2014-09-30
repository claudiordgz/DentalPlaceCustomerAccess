using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DentalPlace.Core.UX
{
    public delegate void loadConfigurationFileCallback();
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class MenuBar : UserControl
    {
        bool isMenuCollapsed;
        public delegate void OpenFile();
        public delegate void LoadRules();
        public OpenFile openFile { get; set; }
        public LoadRules loadRules { get; set; }

        public MenuBar()
        {
            InitializeComponent();

            ToggleButton.Visibility = System.Windows.Visibility.Visible;
            MainMenu.Visibility = System.Windows.Visibility.Collapsed;
            isMenuCollapsed = true;
            this.DataContext = this;
        }

        private void ExitApplicationClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CollapseMenu();
        }

        private void CollapseMenu()
        {
            if (!isMenuCollapsed)
            {
                MainMenu.Visibility = System.Windows.Visibility.Collapsed;
                ToggleButton.Visibility = System.Windows.Visibility.Visible;
                isMenuCollapsed = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuCollapsed)
            {
                MainMenu.Visibility = System.Windows.Visibility.Visible;
                ToggleButton.Visibility = System.Windows.Visibility.Hidden;
                isMenuCollapsed = false;
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (openFile != null)
            {
                if (loadRules != null)
                {
                    openFile();
                    loadRules();
                }
            }
        }
    }
}
