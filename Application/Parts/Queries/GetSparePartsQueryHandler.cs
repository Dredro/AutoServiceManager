using Application.Parts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Parts.Queries;

public record GetSparePartsQuery() : IRequest<IEnumerable<GetSparePartDto>>;

public class GetSparePartsQueryHandler : IRequestHandler<GetSparePartsQuery,IEnumerable<GetSparePartDto>>
{
    private readonly IAppDbContext _dbContext;

    public GetSparePartsQueryHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetSparePartDto>> Handle(GetSparePartsQuery request, CancellationToken cancellationToken)
    {
        var parts = await _dbContext.SpareParts
            .ToListAsync(cancellationToken);
        var partDtos = parts.Select(part => new GetSparePartDto
        {
            Id = part.Id.ToString(),
            CatalogNumber = part.CatalogNumber,
            Name = part.Name,
            Make = part.Make,
            Quality = part.Quality,
            QuantityInStock = part.QuantityInStock,
            Price = part.Price,
            Category = part.Category
        }).ToList();

        return partDtos;
    }
}