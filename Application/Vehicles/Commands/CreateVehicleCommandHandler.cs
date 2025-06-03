using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Vehicles.Commands;

public record CreateVehicleCommand(
 string Make,
 string Model,
 string Vin,
 string? RegistrationNumber,
 DateOnly RegistrationDate,
 int YearOfProduction,
 string? EngineCode,
 int EngineDisplacement,
 decimal Power,
 string ClientId,
 VehicleType Type):IRequest<string>;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, string>
{
 private readonly IAppDbContext _dbContext;

 public CreateVehicleCommandHandler(IAppDbContext dbContext)
 {
  this._dbContext = dbContext;
 }

 public async Task<string> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
 {
  var client =
   await _dbContext.Clients.FindAsync(new object?[] { request.ClientId }, cancellationToken: cancellationToken);
  if (client == null)
   throw new NotFoundException($"Client {request.ClientId} not found");
  var vehicle = new Vehicle
  {
   Make = request.Make,
   Model = request.Model,
   Vin = request.Vin,
   RegistrationNumber = request.RegistrationNumber,
   RegistrationDate = request.RegistrationDate,
   YearOfProduction = request.YearOfProduction,
   EngineCode = request.EngineCode,
   EngineDisplacement = request.EngineDisplacement,
   Power = request.Power,
   Type = request.Type,
   Client = client
  };
  await _dbContext.Vehicles.AddAsync(vehicle, cancellationToken);
  await _dbContext.SaveChangesAsync(cancellationToken);
  return vehicle.Id.ToString();
 }
}