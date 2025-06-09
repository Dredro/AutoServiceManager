using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf.Views.StorageManager
{
    /// <summary>
    /// Logika interakcji dla klasy DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void AddPartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logika dodawania nowej części.", "Dodaj Część");
        }

        private void AvailablePartsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AvailablePartsListBox.SelectedItem != null)
            {

            }
        }

        private void MissingPartsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderMissingPartsButton.IsEnabled = MissingPartsListBox.SelectedItems.Count > 0;
        }

        private void OrderSelectedMissingParts_Click(object sender, RoutedEventArgs e)
        {
            if (MissingPartsListBox.SelectedItems.Count > 0)
            {
                string selectedItems = "Zamawianie następujących części:\n";
                foreach (var item in MissingPartsListBox.SelectedItems)
                {
                    selectedItems += $"- {item.ToString()}\n";
                }
                MessageBox.Show(selectedItems, "Zamów Części");
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null && (tb.Text == "Wyszukaj część..." || tb.Text == "Wyszukaj brakującą część..."))
            {
                tb.Text = string.Empty;
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Foreground = Brushes.Gray;
                if (tb.Name.Contains("Available"))
                {
                    tb.Text = "Wyszukaj część...";
                }
                else
                {
                    tb.Text = "Wyszukaj brakującą część...";
                }
            }
        }
    }
}
