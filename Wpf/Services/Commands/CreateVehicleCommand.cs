using Wpf.Models.DTOs;

namespace Wpf.Services;

public class CreateVehicleCommand
{
   public string Make { get; set; } = string.Empty;
   public string Model { get; set; } = string.Empty;
   public string Vin { get; set; } = string.Empty; 
   public string? RegistrationNumber { get; set; }
   public DateOnly RegistrationDate { get; set; }
   public int YearOfProduction { get; set; }
   public string? EngineCode { get; set; }
   public int EngineDisplacement { get; set; }
   public decimal Power { get; set; }
   public string ClientId { get; set; } = string.Empty;
   public VehicleType Type { get; set; } 
}