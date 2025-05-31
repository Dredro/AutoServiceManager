using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Services.Commands;

public record EditServiceCommand(string Id) : IRequest<string>
{
   public string? Name { get; set; }
   public string? Description { get; set; }
   public decimal? MinimalPrice  { get; set; }
   public decimal? MaximalPrice { get; set; }
}
public class EditServiceCommandHandler : IRequestHandler<EditServiceCommand,string>
{
   private readonly IAppDbContext _dbContext;

   public EditServiceCommandHandler(IAppDbContext dbContext)
   {
      _dbContext = dbContext;
   }

   public async Task<string> Handle(EditServiceCommand request, CancellationToken cancellationToken)
   {
      if (!Guid.TryParse(request.Id, out var serviceId))
      {
         throw new ValidationException("Invalid service ID format.");
      }

      var service = await _dbContext.Services.FindAsync(serviceId, cancellationToken);

      if (service == null)
      {
         throw new Exceptions.NotFoundException($"Service with id {request.Id} not found");
      }

      if (request.Name != null)
      {
         service.Name = request.Name;
      }

      if (request.Description != null)
      {
         service.Description = request.Description;
      }

      if (request.MinimalPrice.HasValue)
      {
         service.MinimalPrice = request.MinimalPrice.Value;
      }

      if (request.MaximalPrice.HasValue)
      {
         service.MaximalPrice = request.MaximalPrice.Value;
      }

      if (service.MinimalPrice > service.MaximalPrice)
      {
         throw new ValidationException("Minimal price cannot be greater than maximal price.");
      }

      await _dbContext.SaveChangesAsync(cancellationToken);

      return service.Id.ToString();
   }
}