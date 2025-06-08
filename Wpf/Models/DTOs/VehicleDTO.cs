using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public enum VehicleType
{
    Car,
    Motorcycle,
    Truck,
}

public class VehicleDTO
{
    public Guid Id { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Vin { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public int YearOfProduction { get; set; }
    public string? EngineCode { get; set; }
    public int EngineDisplacement { get; set; }
    public decimal Power { get; set; }
    public VehicleType Type { get; set; }
    public ClientDTO? Client { get; set; }
    public ICollection<OrderDTO> Orders { get; set; } = [];
}
