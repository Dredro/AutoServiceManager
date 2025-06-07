using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clients.Commands;

public record AddClientCommand
(
string FirstName,
string LastName,
string Email,
string PhoneNumber
    ) : IRequest<string>;

public class AddClientCommandHandler : IRequestHandler<AddClientCommand,string>
{
    private readonly IAppDbContext _dbContext;

    public AddClientCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

     public async Task<string> Handle(AddClientCommand request, CancellationToken cancellationToken)
     {
         var personalInfo = new PersonalInfo
         {
             FirstName = request.FirstName,
             LastName = request.LastName,
             Email = request.Email,
             PhoneNumber = request.PhoneNumber
         };
        if(_dbContext.Clients.Any(c=>c.PersonalInfo == personalInfo))
        {
            throw new InvalidOperationException("Client with the same personal info already exists.");
        }

        var client = new Client
        {
            PersonalInfo = personalInfo
        };
        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return client.Id.ToString();
    }
}