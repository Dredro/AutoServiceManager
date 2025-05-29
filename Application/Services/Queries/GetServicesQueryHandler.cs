using Application.Services.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Queries;

public record GetServicesQuery() : IRequest<IEnumerable<GetServiceDto>>;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery,IEnumerable<GetServiceDto>>
{
    private readonly IAppDbContext _appDbContext;

    public GetServicesQueryHandler(IAppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<GetServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var services = await _appDbContext.Services
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var servicesDto = services.Select(service => new GetServiceDto
        {
            Name = service.Name,
            Description = service.Description,
            Id = service.Id.ToString(), 
            MaximalPrice = service.MaximalPrice,
            MinimalPrice = service.MinimalPrice
        }).ToList(); 

        return servicesDto;
    }
}