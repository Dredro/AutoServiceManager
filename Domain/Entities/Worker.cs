using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Worker : Entity
{
    public required PersonalInfo PersonalInfo { get; set; }
    public decimal? Salary { get; set; }
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<Notification> SentNotifications { get; set; } = [];
    public ICollection<ServiceInProgress> ServiceInProgress { get; set; } = [];
    //room for roles and data to authentication if it should be here
    

}