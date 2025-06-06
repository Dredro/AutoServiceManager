using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class OrderSparePartDTO
{
    public Guid OrderId { get; set; }
    public OrderDTO? Order { get; set; }
    public Guid ProductId { get; set; }
    public SparePartsDTO? SparePart { get; set; }
    public int Quantity { get; set; }
}
