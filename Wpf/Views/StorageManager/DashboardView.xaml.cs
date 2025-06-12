using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.ViewModels.StorageManager; // Ensure this using is present

namespace Wpf.Views.StorageManager
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
          }

        // Event handlers for the hint text behavior
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is StorageManagerDashboardViewModel viewModel && sender is TextBox textBox)
            {
                viewModel.SearchTextBoxGotFocusCommand.Execute(textBox.Tag?.ToString());
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is StorageManagerDashboardViewModel viewModel && sender is TextBox textBox)
            {
                viewModel.SearchTextBoxLostFocusCommand.Execute(textBox.Tag?.ToString());
            }
        }
        
    }
}