using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class WorkerDTO
{
    public required PersonalInfo PersonalInfo { get; set; }
    public decimal? Salary { get; set; }
    public ICollection<NotificationDTO> Notifications { get; set; } = [];
    public ICollection<NotificationDTO> SentNotifications { get; set; } = [];
    public ICollection<ServiceInProgressDTO> ServiceInProgress { get; set; } = [];
}
