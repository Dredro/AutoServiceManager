using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class OrderDTO
{
    public bool IsPaid { get; set; }
    public DateOnly? FinalizationDate { get; set; }
    public required ClientDTO Client { get; set; }
    public VehicleDTO? Vehicle { get; set; }
    public ICollection<ServiceInProgressDTO> ServicesToDo { get; set; } = [];
    public ICollection<OrderSparePartDTO> SpareParts { get; set; } = [];
}
