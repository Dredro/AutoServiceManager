using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Models.DTOs;

public class OrderDTO
{
    public Guid Id { get; set; }
    public bool IsPaid { get; set; }
    public DateOnly? FinalizationDate { get; set; }
    public ClientDTO? Client { get; set; } 
    public VehicleDTO? Vehicle { get; set; }
    public ICollection<ServiceInProgressDTO> ServicesToDo { get; set; } = new List<ServiceInProgressDTO>();
    public ICollection<OrderSparePartDTO> SpareParts { get; set; } = new List<OrderSparePartDTO>();
    public string ClientId { get; set; }
    public string? VehicleId { get; set; }
    public OrderDTO()
    {
        
    }

    public string Status => GetOrderStatus();
    public decimal TotalCost => CalculateTotalCost();

    private string GetOrderStatus()
    {
        if (IsPaid && FinalizationDate.HasValue)
            return "Zakończone";

        if (ServicesToDo?.Any() == true)
        {
            var allCompleted = ServicesToDo.All(s => s.ServiceStatus == ServiceStatus.Completed);
            if (allCompleted)
                return IsPaid ? "Zakończone" : "Oczekuje na płatność";

            var anyInProgress = ServicesToDo.Any(s => s.ServiceStatus == ServiceStatus.InProgress);
            if (anyInProgress)
                return "W trakcie";

            return "Oczekuje na rozpoczęcie";
        }

        return "Nowe";
    }

    private decimal CalculateTotalCost()
    {
        decimal servicesCost = ServicesToDo?.Sum(s => s.Price ?? 0) ?? 0;
        decimal partsCost = SpareParts?.Sum(p => (p.SparePart?.Price ?? 0) * p.Quantity) ?? 0;
        return servicesCost + partsCost;
    }
}
