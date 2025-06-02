using Application.Clients.DTOs;
using Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clients.Queries;

public record GetClientQuery(string Id) : IRequest<GetClientDto>;

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, GetClientDto>
{
    private readonly IAppDbContext _dbContext;

    public GetClientQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetClientDto> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var clientId))
        {
            throw new ValidationException(nameof(request.Id), "Invalid Client ID format.");
        }

        var client = await _dbContext.Clients
            .Include(c => c.Vehicles) 
            .Include(c => c.Orders)  
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken);

        if (client == null)
        {
            throw new NotFoundException($"Client with id{request.Id} not found!");
        }

        return new GetClientDto
        {
            Id = client.Id.ToString(),
            FirstName = client.PersonalInfo.FirstName,
            LastName = client.PersonalInfo.LastName,
            Email = client.PersonalInfo.Email,
            PhoneNumber = client.PersonalInfo.PhoneNumber,
            VehiclesIds = client.Vehicles.Select(v => v.Id.ToString()).ToList(),
            OrdersIds = client.Orders.Select(o => o.Id.ToString()).ToList()
        };
    }
}
