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

namespace Wpf.ViewModel.Worker;

public class WorkerDashboardViewModel
{
    public WorkerDashboardModel DataModel { get; set; } = new();

    public ICommand AddNewOrderCommand { get; set; }


    public WorkerDashboardViewModel()
    {
        DataModel.ActiveOrders = 3;
        DataModel.ServicesProvided = 12;
        DataModel.OrdersToday = 2;

        AddNewOrderCommand = new RelayCommand(TestFunc);
    }

    private void TestFunc()
    {
        DataModel.ActiveOrders++;
        DataModel.ServicesProvided++;
        DataModel.OrdersToday++;
        MessageBox.Show("ADDING STUFF", "INFO");
    }
}
