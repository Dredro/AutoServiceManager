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
    public decimal? Price { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public required ServiceStatus ServiceStatus { get; set; }
    public required ServiceDTO Service { get; set; }
    public required OrderDTO Order { get; set; }
    public ICollection<WorkerDTO> Workers { get; set; } = [];
}
