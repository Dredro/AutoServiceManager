using Wpf.Models.DTOs;

namespace Wpf.Services;

public class CreateSparePartCommand
{
    public string? CatalogNumber { get; set; }
    public string? Name { get; set; }
    public string? Make { get; set; }
    public char Quality { get; set; }
    public int QuantityInStock { get; set; }
    public decimal Price { get; set; }
    public PartCategory Category { get; set; }
}