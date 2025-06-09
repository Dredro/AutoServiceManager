using Application.Vehicles.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Vehicles.Queries;

public record GetAllVehiclesQuery() : IRequest<List<GetVehicleDto>>;

public class GetAllVehiclesQueryHandler : IRequestHandler<GetAllVehiclesQuery, List<GetVehicleDto>>
{
    private readonly IAppDbContext _context;

    public GetAllVehiclesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetVehicleDto>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Client) 
            .Include(v => v.Orders) 
            .ToListAsync(cancellationToken) 
            .ConfigureAwait(false);
        
        var dtos = vehicles.Select(vehicle =>
        {
            return new GetVehicleDto
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
        }).ToList();

        return dtos;
    }
}