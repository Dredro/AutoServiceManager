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
using Wpf.Models;
using Wpf.Models.DTOs;
using Wpf.Services;

namespace Wpf.ViewModel.Worker;

public class WorkerDashboardViewModel : BaseViewModel
{
    private readonly OrderService _orderService;
    private readonly ServiceService _serviceService;

    private WorkerDashboardModel _dataModel = new WorkerDashboardModel();
    public WorkerDashboardModel DataModel
    {
        get => _dataModel;
        set
        {
            _dataModel = value;
            OnPropertyChanged(nameof(_dataModel));
        }
    }

    public WorkerDashboardViewModel(OrderService orderService, ServiceService serviceService)
    {
        _orderService = orderService;
        _serviceService = serviceService;

        LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        await LoadActiveOrders();
        await LoadServices();
        await LoadStatistics();
    }

    private async Task LoadActiveOrders()
    {
        var orders = await _orderService.GetOrdersWithFiltersAsync(isPaid: false);
        if (orders != null)
        {
            DataModel.Orders = new ObservableCollection<OrderDTO>(orders);
            DataModel.OnPropertyChanged(nameof(DataModel.Orders));
        }
    }

    private async Task LoadServices()
    {
        var services = await _serviceService.GetServicesAsync();
        if (services != null)
        {
            DataModel.Services = new ObservableCollection<ServiceDTO>(services);
            DataModel.ServicesProvided = services.Count;
            DataModel.OnPropertyChanged(nameof(DataModel.Services));
        }
    }

    private async Task LoadStatistics()
    {
        var allOrders = await _orderService.GetOrdersAsync();
        if (allOrders != null)
        {
            // Aktywne zlecenia (nieopłacone)
            DataModel.ActiveOrders = allOrders.Count(o => !o.IsPaid);

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Zlecenia dzisiaj (utworzone dzisiaj)
            DataModel.OrdersToday = allOrders.Count(o =>
                o.FinalizationDate <= today || o.FinalizationDate == null);
        }
    }
}
