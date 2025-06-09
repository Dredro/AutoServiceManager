using Wpf.Models.DTOs;

namespace Wpf.Services;

public class CreateSparePartCommand
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public PartCategory Category { get; set; }
}