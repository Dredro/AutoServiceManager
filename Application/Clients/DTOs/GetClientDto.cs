namespace Application.Clients.DTOs;

public class GetClientDto
{
    public string Id {get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> VehiclesIds { get; set; }
    public List<string> OrdersIds { get; set; }
}