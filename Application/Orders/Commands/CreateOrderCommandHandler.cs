using Application.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands;

public record CreateOrderCommand(
    string ClientId,
    string? VehicleId,
    List<string> ServicesToDoIds,
    List<string> SparePartsIds
    ) : IRequest<string>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
{
    private readonly IAppDbContext _context;

    public CreateOrderCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ClientId, out var clientIdGuid))
        {
            throw new BadRequestException($"Invalid ClientId format: {request.ClientId}");
        }
        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == clientIdGuid, cancellationToken)
            .ConfigureAwait(false);
        if (client == null)
        {
            throw new NotFoundException($"Client with ID {request.ClientId} not found.");
        }
        
        Vehicle? vehicle = null; 
        Guid? vehicleIdGuid = null;
        if (!string.IsNullOrEmpty(request.VehicleId))
        {
            if (!Guid.TryParse(request.VehicleId, out var parsedVehicleId))
            {
                throw new BadRequestException($"Invalid VehicleId format: {request.VehicleId}");
            }
            vehicleIdGuid = parsedVehicleId;
            vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleIdGuid, cancellationToken)
                .ConfigureAwait(false);
            if (vehicle == null)
            {
                throw new NotFoundException($"Vehicle with ID {request.VehicleId} not found.");
            }
        }
        
        var servicesToAdd = new List<ServiceInProgress>();
        if (request.ServicesToDoIds != null && request.ServicesToDoIds.Any())
        {
            var serviceGuids = new List<Guid>();
            foreach (var idStr in request.ServicesToDoIds)
            {
                if (!Guid.TryParse(idStr, out var guid))
                    throw new BadRequestException($"Invalid ServiceToDoId format: {idStr}");
                serviceGuids.Add(guid);
            }

            servicesToAdd = await _context.ServiceInProgresses
                .Where(s => serviceGuids.Contains(s.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (servicesToAdd.Count != serviceGuids.Count)
            {
                var foundIds = servicesToAdd.Select(s => s.Id).ToList();
                var missingIds = serviceGuids.Except(foundIds).Select(id => id.ToString());
                throw new NotFoundException($"Following services not found: {string.Join(", ", missingIds)}");
            }
        }

        var sparePartsToAdd = new List<OrderSparePart>();
        if (request.SparePartsIds != null && request.SparePartsIds.Any())
        {
            var sparePartGuids = new List<Guid>();
            foreach (var idStr in request.SparePartsIds)
            {
                if (!Guid.TryParse(idStr, out var guid))
                    throw new BadRequestException($"Invalid SparePartId format: {idStr}");
                sparePartGuids.Add(guid);
            }

            sparePartsToAdd = await _context.OrderSpareParts
                .Where(sp => sparePartGuids.Contains(sp.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (sparePartsToAdd.Count != sparePartGuids.Count)
            {
                var foundIds = sparePartsToAdd.Select(sp => sp.Id).ToList();
                var missingIds = sparePartGuids.Except(foundIds).Select(id => id.ToString());
                throw new NotFoundException($"Following spare parts not found: {string.Join(", ", missingIds)}");
            }
        }

        var order = new Order
        {
           IsPaid = request.IsPaid,
           FinalizationDate = request.FinalizationDate,
           Client = client,
           Vehicle = vehicle,
           SpareParts = sparePartsToAdd,
           ServicesToDo = servicesToAdd,
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return order.Id.ToString();
    }
}