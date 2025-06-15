using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Vehicles.Commands;

public record EditVehicleCommand(
    string Id, 
    string? Make,
    string? Model,
    string? Vin,
    string? RegistrationNumber,
    DateOnly? RegistrationDate,
    int? YearOfProduction,
    string? EngineCode,
    int? EngineDisplacement,
    decimal? Power,
    string? ClientId 
) : IRequest;

public class EditVehicleCommandHandler : IRequestHandler<EditVehicleCommand>
{
    private readonly IAppDbContext _context;

    public EditVehicleCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EditVehicleCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out Guid vehicleId))
        {
            throw new ArgumentException($"Invalid Vehicle ID format: {request.Id}", nameof(request.Id));
        }

        var vehicle = await _context.Vehicles
                                    .Include(v => v.Client) 
                                    .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

        if (vehicle == null)
        {
            throw new KeyNotFoundException($"Vehicle with ID {request.Id} not found.");
        }

        if (request.Make != null)
        {
            vehicle.Make = request.Make;
        }
        if (request.Model != null)
        {
            vehicle.Model = request.Model;
        }
        if (request.Vin != null)
        {
            vehicle.Vin = request.Vin;
        }
        if (request.RegistrationNumber != null)
        {
            vehicle.RegistrationNumber = request.RegistrationNumber;
        }
        if (request.RegistrationDate.HasValue) 
        {
            vehicle.RegistrationDate = request.RegistrationDate.Value;
        }
        if (request.YearOfProduction.HasValue)
        {
            vehicle.YearOfProduction = request.YearOfProduction.Value;
        }
        if (request.EngineCode != null)
        {
            vehicle.EngineCode = request.EngineCode;
        }
        if (request.EngineDisplacement.HasValue)
        {
            vehicle.EngineDisplacement = request.EngineDisplacement.Value;
        }
        if (request.Power.HasValue)
        {
            vehicle.Power = request.Power.Value;
        }
        
        if (request.ClientId != null) 
        {
            if (!Guid.TryParse(request.ClientId, out Guid clientId))
            {
                throw new ArgumentException($"Invalid Client ID format: {request.ClientId}", nameof(request.ClientId));
            }
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");
            }
            vehicle.Client = client; 
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}