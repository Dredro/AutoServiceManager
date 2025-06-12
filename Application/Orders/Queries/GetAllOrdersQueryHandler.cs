using Application.Orders.DTOs;
using Application.Workers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Clients.DTOs;
using Application.Vehicles.DTOs;
using Domain.Enums;

namespace Application.Orders.Queries
{
    public record GetAllOrdersQuery : IRequest<IEnumerable<OrderDto>>;

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDto>>
    {
        private readonly IAppDbContext _context;

        public GetAllOrdersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                // Include related entities to avoid lazy loading and null references
                .Include(o => o.Client)
                .Include(o => o.Vehicle)
                .Include(o => o.ServicesToDo)
                    .ThenInclude(s => s.Workers)
                        .ThenInclude(w => w.PersonalInfo)
                .Include(o => o.SpareParts)
                    .ThenInclude(sp => sp.SparePart) // Assuming SparePart is a navigation property on the link entity
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                ClientId = order.Client?.Id.ToString() ?? string.Empty,
                VehicleId = order.Vehicle?.Id.ToString() ?? string.Empty,
                IsPaid = order.IsPaid,
                FinalizationDate = order.FinalizationDate,
                Client = new GetClientDto
                {
                    Id = order.Client?.Id.ToString() ?? string.Empty,
                    FirstName = order.Client?.PersonalInfo?.FirstName ?? string.Empty,
                    LastName = order.Client?.PersonalInfo?.LastName ?? string.Empty,
                    Email = order.Client?.PersonalInfo?.Email ?? string.Empty,
                    PhoneNumber = order.Client?.PersonalInfo?.PhoneNumber ?? string.Empty,
                    VehiclesIds = null, // or set as appropriate
                    OrdersIds = null    // or set as appropriate
                },
                Vehicle = new GetVehicleDto
                {
                    Id = order.Vehicle?.Id.ToString() ?? string.Empty,
                    Make = order.Vehicle?.Make ?? string.Empty,
                    Model = order.Vehicle?.Model ?? string.Empty,
                    RegistrationNumber = order.Vehicle?.RegistrationNumber ?? string.Empty,
                    Vin = order.Vehicle?.Vin ?? string.Empty,
                    ClientId = string.Empty,
                    Type = order.Vehicle?.Type ?? VehicleType.Car,
                },

                ServicesToDo = order.ServicesToDo?.Select(service => new ServiceInProgressDto
                {
                    Id = service.Id,
                    Price = service.Price,
                    StartDate = service.StartDate,
                    EndDate = service.EndDate,
                    ServiceStatus = service.ServiceStatus,
                    ServiceId = service.Id.ToString(),
                    Workers = service.Workers?.Select(worker => new GetWorkerDto(
                        worker.Id.ToString(),
                        worker.PersonalInfo?.FirstName ?? string.Empty,
                        worker.PersonalInfo?.LastName ?? string.Empty,
                        worker.PersonalInfo?.Email ?? string.Empty,
                        worker.PersonalInfo?.PhoneNumber ?? string.Empty,
                        worker.Salary,
                        worker.ServiceInProgress?.Select(sip => sip.Id.ToString()).ToList() ?? new List<string>()
                    )).ToList() ?? new List<GetWorkerDto>()
                }).ToList() ?? new List<ServiceInProgressDto>(),
                SpareParts = order.SpareParts?.Select(orderSparePartLink => new OrderSparePartDto
                {
                    Id = orderSparePartLink.SparePart?.Id.ToString() ?? string.Empty,
                    Quantity = orderSparePartLink.Quantity,
                    Price = _context.SpareParts.FirstOrDefault(s=>orderSparePartLink.SparePart != null && s.Id == orderSparePartLink.SparePart.Id)?.Price ?? 0,
                }).ToList() ?? new List<OrderSparePartDto>()
            }).ToList();

            return orderDtos;
        }
    }
}
