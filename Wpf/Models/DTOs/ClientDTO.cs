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
    public Guid Id { get; set; }
    public PersonalInfo? PersonalInfo { get; set; }
    public ICollection<VehicleDTO> Vehicles { get; set; } = [];
    public ICollection<OrderDTO> Orders { get; set; } = [];
    public ClientDTO()
    {
        
    }
}
