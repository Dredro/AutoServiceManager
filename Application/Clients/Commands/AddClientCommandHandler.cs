using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clients.Commands;

public record AddClientCommand
(
string FirstName,
string LastName,
string Email,
string PhoneNumber,
List<string> VehiclesIds,
List<string> OrdersIds 
    ) : IRequest<string>;

public class AddClientCommandHandler : IRequestHandler<AddClientCommand,string>
{
    private readonly IAppDbContext _dbContext;

    public AddClientCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

     public async Task<string> Handle(AddClientCommand request, CancellationToken cancellationToken)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var vehicleGuids = new List<Guid>();
        foreach (var idString in request.VehiclesIds)
        {
            if (Guid.TryParse(idString, out var guid))
            {
                vehicleGuids.Add(guid);
            }
            else
            {
                if (!validationErrors.ContainsKey(nameof(request.VehiclesIds)))
                    validationErrors[nameof(request.VehiclesIds)] = new string[] { $"Invalid GUID format for vehicle ID: {idString}" };
                else
                    validationErrors[nameof(request.VehiclesIds)] = validationErrors[nameof(request.VehiclesIds)].Append($"Invalid GUID format for vehicle ID: {idString}").ToArray();
            }
        }

        var orderGuids = new List<Guid>();
        foreach (var idString in request.OrdersIds)
        {
            if (Guid.TryParse(idString, out var guid))
            {
                orderGuids.Add(guid);
            }
            else
            {
                if (!validationErrors.ContainsKey(nameof(request.OrdersIds)))
                    validationErrors[nameof(request.OrdersIds)] = new string[] { $"Invalid GUID format for order ID: {idString}" };
                else
                    validationErrors[nameof(request.OrdersIds)] = validationErrors[nameof(request.OrdersIds)].Append($"Invalid GUID format for order ID: {idString}").ToArray();
            }
        }

        if (validationErrors.Any())
        {
            throw new Exceptions.ValidationException(validationErrors);
        }

        var existingVehicles = await _dbContext.Vehicles
            .Where(v => vehicleGuids.Contains(v.Id))
            .ToListAsync(cancellationToken);

        var foundVehicleIds = existingVehicles.Select(v => v.Id).ToList();
        var missingVehicleIds = vehicleGuids.Except(foundVehicleIds).ToList();

        if (missingVehicleIds.Any())
        {
            validationErrors.Add(nameof(request.VehiclesIds),
                new string[] { $"The following vehicle IDs were not found: {string.Join(", ", missingVehicleIds)}" });
        }

        var existingOrders = await _dbContext.Orders
            .Where(o => orderGuids.Contains(o.Id))
            .ToListAsync(cancellationToken);

        var foundOrderIds = existingOrders.Select(o => o.Id).ToList();
        var missingOrderIds = orderGuids.Except(foundOrderIds).ToList();

        if (missingOrderIds.Any())
        {
            validationErrors.Add(nameof(request.OrdersIds),
                new string[] { $"The following order IDs were not found: {string.Join(", ", missingOrderIds)}" });
        }

        if (validationErrors.Any())
        {
            throw new Exceptions.ValidationException(validationErrors);
        }

        var personalInfo = new PersonalInfo
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var client = new Client
        {
            PersonalInfo = personalInfo,
            Vehicles = existingVehicles,
            Orders = existingOrders
        };

        _dbContext.Clients.Add(client);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return client.Id.ToString();
    }
}