using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Parts.Commands;

public record CreateSparePartCommand(
        string CatalogNumber,
        string Name,
        string Make,
        char Quality,
        int QuantityInStock,
        decimal Price,
        PartCategory Category
    ) : IRequest<string>;

public class CreateSparePartCommandHandler : IRequestHandler<CreateSparePartCommand,string>
{
    private readonly IAppDbContext _dbContext;

    public CreateSparePartCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(CreateSparePartCommand request, CancellationToken cancellationToken)
    {
        var part = new SparePart
        {
            CatalogNumber = request.CatalogNumber,
            Name = request.Name,
            Make = request.Make,
            Quality = request.Quality,
            QuantityInStock = request.QuantityInStock,
            Price = request.Price,
            Category = request.Category
        };
        await _dbContext.SpareParts.AddAsync(part, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return part.Id.ToString();
    }
}