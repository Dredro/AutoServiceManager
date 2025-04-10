using Microsoft.EntityFrameworkCore;

namespace Domain.ValueObjects;

[Owned]
public class PersonalInfo
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}