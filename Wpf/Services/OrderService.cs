using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wpf.Models.DTOs;
using Wpf.ViewModel;

namespace Wpf.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
    Task<OrderDTO> GetOrderByIdAsync(Guid id);
    Task AddOrderAsync(OrderDTO order);
    Task UpdateOrderAsync(OrderDTO order);
    Task DeleteOrderAsync(Guid id);
}

public interface IDialogService
{
    bool? ShowDialog(BaseViewModel viewModel);
}