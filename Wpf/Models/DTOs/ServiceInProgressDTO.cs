using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public enum ServiceStatus
{
    PendingForStart,
    PendingForParts,
    InProgress,
    Completed,
}

public class ServiceInProgressDTO
{
    public Guid Id { get; set; }
    public decimal? Price { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ServiceStatus ServiceStatus { get; set; }
    public ServiceDTO? Service { get; set; }
    public OrderDTO? Order { get; set; }
    public ICollection<WorkerDTO> Workers { get; set; } = [];
}
