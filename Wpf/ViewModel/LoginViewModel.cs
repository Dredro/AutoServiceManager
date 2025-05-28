using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wpf.Core;

namespace Wpf.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event Action? LoginSucceeded;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            bool authenticated = true;

            if (authenticated)
                LoginSucceeded?.Invoke();
        }
    }
}
