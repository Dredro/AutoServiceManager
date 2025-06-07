using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;


namespace Wpf.Models;

public class WorkerDashboardModel : INotifyPropertyChanged
{
    private int _activeOrders;
    private int _servicesProvided;
    private int _ordersToday;

    public int ActiveOrders
    {
        get => _activeOrders;
        set => SetProperty(ref _activeOrders, value);
    }

    public int ServicesProvided
    {
        get => _servicesProvided;
        set => SetProperty(ref _servicesProvided, value);
    }

    public int OrdersToday
    {
        get => _ordersToday;
        set => SetProperty(ref _ordersToday, value);
    }

    public ObservableCollection<OrderDTO> Orders { get; set; } = new();
    public ObservableCollection<ServiceInProgressDTO> Services { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
