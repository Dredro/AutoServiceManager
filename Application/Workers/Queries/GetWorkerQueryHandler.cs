using Application.Exceptions;
using Application.Workers.DTOs;
using MediatR;

namespace Application.Workers.Queries;

public record GetWorkerQuery(string Id) : IRequest<GetWorkerDto>;

public class GetWorkerQueryHandler : IRequestHandler<GetWorkerQuery, GetWorkerDto>
{
    private readonly IAppDbContext _context;

    public GetWorkerQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetWorkerDto> Handle(GetWorkerQuery request, CancellationToken cancellationToken)
    {
        var worker = await _context.Workers.FindAsync(new object?[] { Guid.Parse(request.Id) }, cancellationToken: cancellationToken);
        if(worker == null)
            throw new NotFoundException($"Worker with Id: {request.Id} not found");
        var dto = new GetWorkerDto
        (
            worker.PersonalInfo.FirstName,
            worker.PersonalInfo.LastName,
            worker.PersonalInfo.Email,
            worker.PersonalInfo.PhoneNumber,
            worker.Salary
        );
        return dto;
    }
}