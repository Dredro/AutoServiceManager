using Wpf.Models.DTOs;

namespace Wpf.Services;

public class CreateVehicleCommand
{
    public int ClientId { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public string? LicensePlate { get; set; }
    public VehicleType Type { get; set; }
}