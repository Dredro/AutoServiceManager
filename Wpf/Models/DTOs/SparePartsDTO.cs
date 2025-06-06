using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public enum PartCategory
{
    Powertrain,
    Electrical,
    Cooling,
    BrakingSystem,
    Suspension,
    Electronics,
    Fuel,
    Exhaust,
    Body,
    Interior,
    Consumables,
    Other,
}

public class SparePartsDTO
{
    public required string CatalogNumber { get; set; }
    public required string Name { get; set; }
    public required string Make { get; set; }
    public char Quality { get; set; }
    public int QuantityInStock { get; set; }
    public decimal Price { get; set; }
    public required PartCategory Category { get; set; }
    public ICollection<OrderSparePartDTO> PartsAssignedToOrder { get; set; } = [];
}
