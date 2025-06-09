using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Models.DTOs;

namespace Wpf.ViewModel.Worker;

public class OrderDetailsDialogViewModel : BaseViewModel
{
    public OrderDetailsDialogViewModel(OrderDTO order)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    public OrderDTO Order { get; }
    public string Title => $"Szczegóły zlecenia #{Order.Id}";
}
