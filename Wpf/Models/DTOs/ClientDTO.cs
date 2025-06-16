using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class PersonalInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

public class ClientDTO
{
    public Guid Id {get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> VehiclesIds { get; set; }
    public List<string> OrdersIds { get; set; }
    
    public string FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }
    public override string ToString()
    {
        return $"{FirstName} {LastName} (ID: {Id})";
    }

    public override bool Equals(object? obj)
    {
        return obj is ClientDTO dTO &&
               Id == dTO.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}
