using Application.Exceptions;
using Application.Parts.DTOs;
using MediatR;

namespace Application.Parts.Queries;

public record GetSparePartQuery(string Id) : IRequest<GetSparePartDto>; 
public class GetSparePartQueryHandler : IRequestHandler<GetSparePartQuery,GetSparePartDto>
{
    private readonly IAppDbContext _dbContext;

    public GetSparePartQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetSparePartDto> Handle(GetSparePartQuery request, CancellationToken cancellationToken)
    {
        var part = await _dbContext.SpareParts.FindAsync(Guid.Parse(request.Id), cancellationToken);

        if (part == null)
        {
            throw new NotFoundException($"Part with ID {request.Id} not found!");
        }

        var partDto = new GetSparePartDto
        {
            Id = part.Id.ToString(),
            CatalogNumber = part.CatalogNumber,
            Name = part.Name,
            Make = part.Make,
            Quality = part.Quality,
            QuantityInStock = part.QuantityInStock,
            Price = part.Price,
            Category = part.Category
        };

        return partDto;
    }
}