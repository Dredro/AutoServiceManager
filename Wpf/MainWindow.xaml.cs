using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Wpf.ViewModel;
using Wpf.Views;

namespace Wpf
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            NavigateToDashboard(null, null); // Przekierowanie na dashboard domyślnie
        }

        private void NavigateToDashboard(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new DirectorDashboardPage());
        }

        private void NavigateToSettings(object sender, RoutedEventArgs e)
        {
          //  MainFrame.Navigate(new SettingsPage());
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // Zakończ sesję / wróć do loginu
                this.Close(); // lub MainFrame.Navigate(new LoginPage())
            }
        }
    }
}