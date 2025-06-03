using Application.Exceptions;
using Application.Vehicles.DTOs;
using MediatR;

namespace Application.Vehicles.Queries;

public record GetVehicleQuery(int VehicleId) : IRequest<GetVehicleDto>;

public class GetVehicleQueryHandler : IRequestHandler<GetVehicleQuery, GetVehicleDto>
{
    private readonly IAppDbContext _context;

    public GetVehicleQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<GetVehicleDto> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles.FindAsync(new object?[] { request.VehicleId }, cancellationToken: cancellationToken);
        if(vehicle == null)
            throw new NotFoundException($"Vehicle {request.VehicleId} not found");
        var dto = new GetVehicleDto
        {
            ClientId = vehicle.Client.Id.ToString(),
            Vin = vehicle.Vin,
            Model = vehicle.Model,
            RegistrationNumber = vehicle.RegistrationNumber,
            RegistrationDate = vehicle.RegistrationDate,
            Power = vehicle.Power,
            Type = vehicle.Type,
            EngineDisplacement = vehicle.EngineDisplacement,
            Id = vehicle.Id.ToString(),
            Make = vehicle.Make,
            YearOfProduction = vehicle.YearOfProduction,
            OrderIds = vehicle.Orders.Select(o => o.Id.ToString()).ToList(),
            EngineCode = vehicle.EngineCode
        };
        return dto;
    }
}