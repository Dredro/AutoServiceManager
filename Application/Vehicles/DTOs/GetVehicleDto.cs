using Domain.Enums;

namespace Application.Vehicles.DTOs;

public record GetVehicleDto
{
    public required string Id { get; init; } 

    public required string Make { get; init; }
    public required string Model { get; init; }
    public required string Vin { get; init; }
    public string? RegistrationNumber { get; init; }
    public DateOnly RegistrationDate { get; init; } 
    public int YearOfProduction { get; init; }
    public string? EngineCode { get; init; }
    public int EngineDisplacement { get; init; } 
    public decimal Power { get; init; } 
    public required VehicleType Type { get; init; }

    public required string ClientId { get; init; } 
    
    public IReadOnlyCollection<string> OrderIds { get; init; } = []; 
}