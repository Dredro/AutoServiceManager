using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class NotificationDTO
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool IsViewed { get; set; }
    public required WorkerDTO Sender { get; set; }
    public ICollection<WorkerDTO> Receivers { get; set; } = [];
}
