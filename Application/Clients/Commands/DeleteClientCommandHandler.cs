using MediatR;

namespace Application.Clients.Commands;

public record DeleteClientCommand(string Id) : IRequest;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand>
{
    private readonly IAppDbContext _context;

    public DeleteClientCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = _context.Clients.FirstOrDefault(c => c.Id.ToString() == request.Id);
        if (client == null)
        {
            throw new KeyNotFoundException($"Client with ID {request.Id} not found.");
        }
        _context.Clients.Remove(client);
        return _context.SaveChangesAsync(cancellationToken);
    }
}