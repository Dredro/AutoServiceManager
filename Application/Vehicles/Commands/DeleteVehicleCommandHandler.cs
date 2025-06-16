using MediatR;

namespace Application.Vehicles.Commands;

public record DeleteVehicleCommand(string Id) : IRequest;

public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand>
{
    private readonly IAppDbContext _dbContext;

    public DeleteVehicleCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.Id, out var vehicleId))
        {
            throw new ArgumentException($"Invalid Vehicle ID format: {request.Id}", nameof(request.Id));
        }

        var vehicle = _dbContext.Vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle == null)
        {
            throw new KeyNotFoundException($"Vehicle with ID {request.Id} not found.");
        }

        _dbContext.Vehicles.Remove(vehicle);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}