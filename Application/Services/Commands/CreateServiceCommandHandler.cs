using Domain.Entities;
using Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Commands;

public record CreateServiceCommand(
    string Name,
    string Description,
    decimal MinimalPrice,
    decimal MaximalPrice
) : IRequest<string>;
public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, string>
{
    private readonly IAppDbContext _dbContext;
    public CreateServiceCommandHandler(IAppDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<string> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
            var service = new Service
            {
                Name = request.Name,
                Description = request.Description,
                MinimalPrice = request.MinimalPrice,
                MaximalPrice = request.MaximalPrice
            };
            await _dbContext.Services.AddAsync(service, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return service.Id.ToString();
    }
}