using Domain.Enums;

namespace Application.Parts.DTOs;

public class GetSparePartDto
{
    public string Id { get; init; }
    public string CatalogNumber { get; init; }
    public string Name { get; init; }
    public string Make { get; init; }
    public char Quality { get; init; }
    public int QuantityInStock { get; init; }
    public decimal Price { get; init; }
    public PartCategory Category { get; init; }
}