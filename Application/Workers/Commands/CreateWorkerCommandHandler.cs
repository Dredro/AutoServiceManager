using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Workers.Commands;

public record CreateWorkerCommand() : IRequest<string>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public decimal? Salary { get; set; }
}

public class CreateWorkerCommandHandler : IRequestHandler<CreateWorkerCommand, string>
{
    private readonly IAppDbContext _context;

    public CreateWorkerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateWorkerCommand request, CancellationToken cancellationToken)
    {
        var worker = new Worker
        {
            PersonalInfo = new PersonalInfo
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
            },
            Salary = request.Salary
        };
        await _context.Workers.AddAsync(worker, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return worker.Id.ToString();
    }
}