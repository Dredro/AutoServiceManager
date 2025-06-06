using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class ServiceDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal MinimalPrice { get; set; }
    public decimal MaximalPrice { get; set; }
    public ICollection<ServiceInProgressDTO> ServicesInProgress { get; set; } = [];
}
