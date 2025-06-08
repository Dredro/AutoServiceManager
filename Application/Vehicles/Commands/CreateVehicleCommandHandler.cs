using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
  if (!Guid.TryParse(request.ClientId, out var clientId))
  {
   throw new BadRequestException($"Invalid ClientId format: {request.ClientId}");
  }
  var client = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId,
   cancellationToken: cancellationToken);
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