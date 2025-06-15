using MediatR;
using Microsoft.EntityFrameworkCore; 


namespace Application.Clients.Commands;

public record EditClientCommand(
    string Id,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber
) : IRequest;

public class EditClientCommandHandler : IRequestHandler<EditClientCommand>
{
    private readonly IAppDbContext _context;

    public EditClientCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EditClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _context.Clients
            .Include(c => c.PersonalInfo) 
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (client == null)
        {
            throw new KeyNotFoundException($"Client with ID {request.Id} not found.");
        }
        
        if (request.FirstName != null)
        {
            client.PersonalInfo.FirstName = request.FirstName;
        }

        if (request.LastName != null)
        {
            client.PersonalInfo.LastName = request.LastName;
        }

        if (request.Email != null)
        {
            client.PersonalInfo.Email = request.Email;
        }

        if (request.PhoneNumber != null)
        {
            client.PersonalInfo.PhoneNumber = request.PhoneNumber;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
    }
}