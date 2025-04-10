using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Client : Entity
{
    public required PersonalInfo PersonalInfo { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}