using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Exceptions; 

namespace Application.Orders.Queries;

public record GetOrderQuery : IRequest<IEnumerable<Order>>
{
    public string? ClientId { get; init; }
    public string? VehicleId { get; init; }
    public bool? IsPaid { get; init; }
    public DateOnly? FinalizationDate { get; init; }
}

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, IEnumerable<Order>>
{
    private readonly IAppDbContext _context;

    public GetOrderQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrderQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Order> ordersQuery = _context.Orders
            .Include(o => o.Client)   
            .Include(o => o.Vehicle)   
            .Include(o => o.ServicesToDo) 
                .ThenInclude(s => s.Service) 
            .Include(o => o.SpareParts) 
                .ThenInclude(sp => sp.SparePart);
        
        if (!string.IsNullOrWhiteSpace(query.ClientId))
        {
            if (!Guid.TryParse(query.ClientId, out var clientIdGuid))
            {
                throw new BadRequestException($"Invalid ClientId format: {query.ClientId}");
            }
            ordersQuery = ordersQuery.Where(o => o.Client.Id == clientIdGuid);
        }
        
        if (!string.IsNullOrWhiteSpace(query.VehicleId))
        {
            if (!Guid.TryParse(query.VehicleId, out var vehicleIdGuid))
            {
                throw new BadRequestException($"Invalid VehicleId format: {query.VehicleId}");
            }
            ordersQuery = ordersQuery.Where(o => o.Vehicle != null && o.Vehicle.Id == vehicleIdGuid);
        }
        
        if (query.IsPaid.HasValue) 
        {
            ordersQuery = ordersQuery.Where(o => o.IsPaid == query.IsPaid.Value);
        }
        
        if (query.FinalizationDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.FinalizationDate > query.FinalizationDate.Value);
        }
        
        return await ordersQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}