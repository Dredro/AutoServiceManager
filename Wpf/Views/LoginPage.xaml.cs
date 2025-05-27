using System.Windows;
using System.Windows.Controls;
using System.Net.Mail; 
using System.Text.RegularExpressions; 

namespace Wpf.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            UpdateLoginButtonState();
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string errorMessage = ValidateEmail(email);
            EmailErrorTextBlock.Text = errorMessage;
            FormErrorMessageTextBlock.Text = ""; 
            UpdateLoginButtonState();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            string errorMessage = ValidatePassword(password);
            PasswordErrorTextBlock.Text = errorMessage;
            FormErrorMessageTextBlock.Text = ""; 
            UpdateLoginButtonState();
        }

        private string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "Email jest wymagany.";
            }
            try
            {
                var addr = new MailAddress(email);
                if (addr.Address == email)
                {
                     string emailPattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
                     if (!Regex.IsMatch(email, emailPattern))
                     {
                        return "Nieprawidłowy format adresu email.";
                     }
                    return string.Empty;
                }
            }
            catch (FormatException)
            {
                return "Nieprawidłowy format adresu email.";
            }
            return "Nieprawidłowy format adresu email."; 
        }

        private string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return "Hasło jest wymagane.";
            }
            if (password.Length < 6)
            {
                return "Hasło musi mieć co najmniej 6 znaków.";
            }
          
             if (!Regex.IsMatch(password, @"[A-Z]")) return "Hasło musi zawierać co najmniej jedną wielką literę.";
             if (!Regex.IsMatch(password, @"[0-9]")) return "Hasło musi zawierać co najmniej jedną cyfrę.";
            return string.Empty; 
        }

        private void UpdateLoginButtonState()
        {
            bool isEmailValid = string.IsNullOrEmpty(ValidateEmail(EmailTextBox.Text));
            bool isPasswordValid = string.IsNullOrEmpty(ValidatePassword(PasswordBox.Password));

            LoginButton.IsEnabled = isEmailValid && isPasswordValid;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            FormErrorMessageTextBlock.Text = ""; 

            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            string emailError = ValidateEmail(email);
            string passwordError = ValidatePassword(password);

            EmailErrorTextBlock.Text = emailError;
            PasswordErrorTextBlock.Text = passwordError;

            UpdateLoginButtonState(); 

            if (!LoginButton.IsEnabled) 
            {
                return;
            }

         
            if (email == "user@example.com" && password == "zaq1@WSXcv")
            {
                MessageBox.Show($"Zalogowano pomyślnie!\nEmail: {email}", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            
                if (NavigationService != null)
                {
                   // NavigationService.Navigate(new HomePage());
                }
            }
            else
            {
                FormErrorMessageTextBlock.Text = "Nieprawidłowy email lub hasło.";
            }
        }
    }
}