using System.Windows.Input;

namespace Wpf.Core;

public interface IAsyncRelayCommand : ICommand
{
    void NotifyCanExecuteChanged();
}
