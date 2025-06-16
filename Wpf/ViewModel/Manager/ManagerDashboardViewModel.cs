using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;

namespace Wpf.ViewModel.Manager
{
    class ManagerDashboardViewModel : BaseViewModel
    {
        public Action? GotoOrders { get; set; }

        public ICommand GoToOrdersCommand { get; set; }

        private OrderService _orderService { get; set; }
        private SparePartService _saprePartService { get; set; }

        public ObservableCollection<OrderDTO> Orders { get; set; } = new();
        public ObservableCollection<SparePartsDTO> SpareParts { get; set; } = new();

        public ManagerDashboardViewModel(OrderService orderService, SparePartService sparePartService) 
        {
            _orderService = orderService;
            _saprePartService = sparePartService;
            GoToOrdersCommand = new RelayCommand(backToOrders);

            LoadActiveOrders();
            LoadActiveSpareParts();
        }

        private async Task LoadActiveOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            if (orders != null)
            {
                Orders = new ObservableCollection<OrderDTO>(orders);
            }
            OnPropertyChanged(nameof(Orders));
        }

        private async Task LoadActiveSpareParts()
        {
            var spareparts = await _saprePartService.GetSparePartsAsync();
            if (spareparts != null)
            {
                SpareParts = new ObservableCollection<SparePartsDTO>(spareparts);
            }
            OnPropertyChanged(nameof(SpareParts));
        }

        private void backToOrders()
        {
            GotoOrders?.Invoke();
        }
    }
}
