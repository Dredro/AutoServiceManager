using Domain.Common;

namespace Domain.Entities;

public class Vehicle : Entity
{
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required string Vin { get; set; }
    public string? RegistrationNumber { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public int YearOfProduction { get; set; }
    public string? EngineCode { get; set; }
    public int EngineDisplacement { get; set; }
    public decimal Power { get; set; }
    public required Client Client { get; set; }
    public ICollection<Order> Orders { get; set; } = [];


}