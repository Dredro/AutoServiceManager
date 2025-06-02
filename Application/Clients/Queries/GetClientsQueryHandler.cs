using Application.Clients.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clients.Queries;

public record GetClientsQuery : IRequest<List<GetClientDto>>;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, List<GetClientDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetClientsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _dbContext.Clients
            .Include(c => c.Vehicles)
            .Include(c => c.Orders)
            .AsNoTracking()
            .Select(client => new GetClientDto
            {
                Id = client.Id.ToString(),
                FirstName = client.PersonalInfo.FirstName,
                LastName = client.PersonalInfo.LastName,
                Email = client.PersonalInfo.Email,
                PhoneNumber = client.PersonalInfo.PhoneNumber,
                VehiclesIds = client.Vehicles.Select(v => v.Id.ToString()).ToList(),
                OrdersIds = client.Orders.Select(o => o.Id.ToString()).ToList()
            })
            .ToListAsync(cancellationToken);

        return clients;
    }
}