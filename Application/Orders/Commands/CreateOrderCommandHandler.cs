using Application.Exceptions;
using Application.Orders.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Commands;

public record CreateOrderCommand(
    bool IsPaid,
    string ClientId,
    string? VehicleId,
    List<string> ServicesToDoIds,
    List<OrderSparePartDto> SpareParts
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

        Guid? vehicleIdGuid = null;
        if (!string.IsNullOrEmpty(request.VehicleId))
        {
            if (!Guid.TryParse(request.VehicleId, out var parsedVehicleId))
            {
                throw new BadRequestException($"Invalid VehicleId format: {request.VehicleId}");
            }
            vehicleIdGuid = parsedVehicleId;
        }

        var requestedServiceGuids = new HashSet<Guid>(); 
        if (request.ServicesToDoIds != null && request.ServicesToDoIds.Any())
        {
            foreach (var idStr in request.ServicesToDoIds)
            {
                if (!Guid.TryParse(idStr, out var serviceGuid))
                {
                    throw new BadRequestException($"Invalid ServiceId format: {idStr}");
                }
                requestedServiceGuids.Add(serviceGuid);
            }
        }

        var requestedSparePartsMap = new Dictionary<Guid, OrderSparePartDto>();
        if (request.SpareParts != null && request.SpareParts.Any())
        {
            foreach (var spDto in request.SpareParts)
            {
                if (!Guid.TryParse(spDto.Id, out var sparePartGuid))
                {
                    throw new BadRequestException($"Invalid SparePartId format: {spDto.Id}");
                }
                if (requestedSparePartsMap.ContainsKey(sparePartGuid))
                {
                    throw new BadRequestException($"Duplicate SparePartId found: {spDto.Id}");
                }
                requestedSparePartsMap[sparePartGuid] = spDto;
            }
        }
        var requestedSparePartGuids = requestedSparePartsMap.Keys.ToHashSet();

        var client = await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == clientIdGuid, cancellationToken)
            .ConfigureAwait(false);

        if (client == null)
        {
            throw new NotFoundException($"Client with ID {request.ClientId} not found.");
        }

        Vehicle? vehicle = null;
        if (vehicleIdGuid.HasValue)
        {
            vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleIdGuid.Value, cancellationToken)
                .ConfigureAwait(false);

            if (vehicle == null)
            {
                throw new NotFoundException($"Vehicle with ID {request.VehicleId} not found.");
            }
        }

        var existingServices = new Dictionary<Guid, Service>();
        if (requestedServiceGuids.Any())
        {
            existingServices = await _context.Services
                .Where(s => requestedServiceGuids.Contains(s.Id))
                .ToDictionaryAsync(s => s.Id, cancellationToken)
                .ConfigureAwait(false);

            var missingServiceIds = requestedServiceGuids.Except(existingServices.Keys).Select(id => id.ToString()).ToList();
            if (missingServiceIds.Any())
            {
                throw new NotFoundException($"The following services were not found: {string.Join(", ", missingServiceIds)}");
            }
        }

        var existingSpareParts = new Dictionary<Guid, SparePart>();
        if (requestedSparePartGuids.Any())
        {
            existingSpareParts = await _context.SpareParts
                .Where(sp => requestedSparePartGuids.Contains(sp.Id))
                .ToDictionaryAsync(sp => sp.Id, cancellationToken)
                .ConfigureAwait(false);

            var missingSparePartIds = requestedSparePartGuids.Except(existingSpareParts.Keys).Select(id => id.ToString()).ToList();
            if (missingSparePartIds.Any())
            {
                throw new NotFoundException($"The following spare parts were not found: {string.Join(", ", missingSparePartIds)}");
            }
        }

        var order = new Order
        {
            IsPaid = request.IsPaid,
            Client = client,
            Vehicle = vehicle
        };

        var servicesInProgress = new List<ServiceInProgress>();
        foreach (var serviceGuid in requestedServiceGuids)
        {
            servicesInProgress.Add(new ServiceInProgress
            {
                ServiceStatus = ServiceStatus.PendingForParts,
                Service = existingServices[serviceGuid],
                Order = order 
            });
        }
        order.ServicesToDo = servicesInProgress;


        var orderSparePartsToAdd = new List<OrderSparePart>();
        foreach (var sparePartGuid in requestedSparePartGuids)
        {
            var sparePartDto = requestedSparePartsMap[sparePartGuid];

            orderSparePartsToAdd.Add(new OrderSparePart
            {
                SparePart = existingSpareParts[sparePartGuid],
                Quantity = sparePartDto.Quantity,
                Order = order 
            });
        }
        order.SpareParts = orderSparePartsToAdd;

        _context.Orders.Add(order);
        
        _context.ServiceInProgresses.AddRange(servicesInProgress);
        _context.OrderSpareParts.AddRange(orderSparePartsToAdd);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return order.Id.ToString();
    }
}