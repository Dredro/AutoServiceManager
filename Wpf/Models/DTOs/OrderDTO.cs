using System.Collections.ObjectModel; 

namespace Wpf.Models.DTOs;

public class OrderDTO 
{
    public Guid Id { get; set; }
    public bool IsPaid { get; set; }
    public DateOnly? FinalizationDate { get; set; }
    public ClientDTO? Client { get; set; }
    public VehicleDTO? Vehicle { get; set; }

    public ObservableCollection<ServiceInProgressDTO> ServicesToDo { get; set; } = new ObservableCollection<ServiceInProgressDTO>();
    public ObservableCollection<OrderSparePartDTO> SpareParts { get; set; } = new ObservableCollection<OrderSparePartDTO>();

    public string ClientId { get; set; }
    public string? VehicleId { get; set; }

    public string? DescriptionOfWork { get; set; }
    public string? Remarks { get; set; }

    public OrderDTO()
    {
        ServicesToDo = new ObservableCollection<ServiceInProgressDTO>();
        SpareParts = new ObservableCollection<OrderSparePartDTO>();
    }

    public string Status => GetOrderStatus();

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

    public decimal TotalCost
    {
        get
        {
            decimal servicesTotal = ServicesToDo?.Sum(s => s.CalculatedPrice) ?? 0m;
            
            decimal partsTotal = SpareParts?.Sum(sp => sp.TotalItemCost) ?? 0m;

            return servicesTotal + partsTotal;
        }
    }
}
