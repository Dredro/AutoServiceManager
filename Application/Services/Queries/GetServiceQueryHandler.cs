using Application.Exceptions;
using Application.Services.DTOs;
using MediatR;

namespace Application.Services.Queries;

public record GetServiceQuery(string Id) : IRequest<GetServiceDto>;

public class GetServiceQueryHandler : IRequestHandler<GetServiceQuery,GetServiceDto>
{
    private readonly IAppDbContext _dbContext;

    public GetServiceQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetServiceDto> Handle(GetServiceQuery request, CancellationToken cancellationToken)
    {
        var service = await _dbContext.Services.FindAsync(Guid.Parse(request.Id), cancellationToken);
        if(service == null) throw new NotFoundException($"Service with {request.Id} not found!");

        var serviceDto = new GetServiceDto
        {
            Name = service.Name,
            Description = service.Description,
            Id = service.Id.ToString(),
            MaximalPrice = service.MaximalPrice,
            MinimalPrice = service.MinimalPrice
        };
        return serviceDto;
    }
}