using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Clients.Commands;

public record CreateClientCommand
(
string FirstName,
string LastName,
string Email,
string PhoneNumber
    ) : IRequest<string>;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand,string>
{
    private readonly IAppDbContext _dbContext;

    public CreateClientCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

     public async Task<string> Handle(CreateClientCommand request, CancellationToken cancellationToken)
     {
         var personalInfo = new PersonalInfo
         {
             FirstName = request.FirstName,
             LastName = request.LastName,
             Email = request.Email,
             PhoneNumber = request.PhoneNumber
         };
         if(await _dbContext.Clients.AnyAsync(c =>
                c.PersonalInfo.FirstName == personalInfo.FirstName &&
                c.PersonalInfo.LastName == personalInfo.LastName &&
                c.PersonalInfo.Email == personalInfo.Email &&
                c.PersonalInfo.PhoneNumber == personalInfo.PhoneNumber, cancellationToken))
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