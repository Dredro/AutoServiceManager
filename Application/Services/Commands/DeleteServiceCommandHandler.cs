using MediatR;

namespace Application.Services.Commands;

public record DeleteServiceCommand(string Id): IRequest;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
{
    private readonly IAppDbContext _context;

    public DeleteServiceCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = _context.Services.FirstOrDefault(s => s.Id.ToString() == request.Id);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with id {request.Id} not found.");
        }
        _context.Services.Remove(service);
        return _context.SaveChangesAsync(cancellationToken);
    }
}