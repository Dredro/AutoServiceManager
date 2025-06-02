namespace Application.Services.DTOs;

public class GetServiceDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal MinimalPrice { get; set; }
    public decimal MaximalPrice { get; set; }
}