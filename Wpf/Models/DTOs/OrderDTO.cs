using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel; // Upewnij się, że masz tę dyrektywę using
using System.ComponentModel; // Dodajemy, bo ServiceInProgressDTO i OrderSparePartDTO implementują INPC

namespace Wpf.Models.DTOs;

public class OrderDTO // OrderDTO nie musi implementować INotifyPropertyChanged, chyba że jego właściwości (jak TotalCost) miałyby się zmieniać dynamicznie BEZ zastępowania całego obiektu w kolekcji Orders
{
    public Guid Id { get; set; }
    public bool IsPaid { get; set; }
    public DateOnly? FinalizationDate { get; set; }
    public ClientDTO? Client { get; set; }
    public VehicleDTO? Vehicle { get; set; }

    // ZMIANA: Zmieniono ICollection na ObservableCollection
    public ObservableCollection<ServiceInProgressDTO> ServicesToDo { get; set; } = new ObservableCollection<ServiceInProgressDTO>();
    public ObservableCollection<OrderSparePartDTO> SpareParts { get; set; } = new ObservableCollection<OrderSparePartDTO>();

    public string ClientId { get; set; }
    public string? VehicleId { get; set; }

    public string? DescriptionOfWork { get; set; }
    public string? Remarks { get; set; }

    public OrderDTO()
    {
        // Upewnij się, że kolekcje są inicjalizowane również w konstruktorze bezparametrowym
        ServicesToDo = new ObservableCollection<ServiceInProgressDTO>();
        SpareParts = new ObservableCollection<OrderSparePartDTO>();
        // Optional: Subscribe to CollectionChanged to update TotalCost? This is more complex and might not be needed if the DTO is static.
        // ServicesToDo.CollectionChanged += Items_CollectionChanged;
        // SpareParts.CollectionChanged += Items_CollectionChanged;
        // Optional: Subscribe to PropertyChanged on items within the collections? Also complex.
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

    // NOWA WŁAŚCIWOŚĆ: Oblicza całkowity koszt zlecenia
    public decimal TotalCost
    {
        get
        {
            // Oblicz sumę kosztów usług
            // Używamy ?.Sum() na wypadek gdyby kolekcja była null (choć zainicjowaliśmy ją w konstruktorze)
            // Używamy CalculatedPrice z ServiceInProgressDTO
            decimal servicesTotal = ServicesToDo?.Sum(s => s.CalculatedPrice) ?? 0m;

            // Oblicz sumę kosztów części (ilość * cena jednostkowa)
            // Używamy TotalItemCost z OrderSparePartDTO
            decimal partsTotal = SpareParts?.Sum(sp => sp.TotalItemCost) ?? 0m;

            return servicesTotal + partsTotal;
        }
    }

    // Jeśli potrzebowałbyś dynamicznego aktualizowania TotalCost w DTO BEZ zastępowania DTO w ViewModelu:
    // private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    // {
    //     // Potrzeba wywołać PropertyChanged dla TotalCost.
    //     // ALE OrderDTO nie implementuje INotifyPropertyChanged w tej chwili.
    //     // Jeśli OrderDTO miałoby być bindowane do widoku i TotalCost miałby się aktualizować
    //     // w czasie rzeczywistym gdy dodajesz/usuwasz usługi/części z kolekcji w DTO,
    //     // OrderDTO musiałoby implementować INotifyPropertyChanged i wywoływać OnPropertyChanged(nameof(TotalCost))
    //     // tutaj i w handlerach PropertyChanged dla elementów kolekcji.
    //     // Dla prostoty zakładamy, że TotalCost jest obliczany gdy DTO jest tworzone lub
    //     // gdy jego właściwość TotalCost jest odczytywana (jak w przypadku bindingu).
    // }
}

// Pozostałe definicje DTO (ServiceInProgressDTO, OrderSparePartDTO, SparePartsDTO, etc.)
// powinny pozostać bez zmian lub z już wprowadzonymi poprawkami z Twoich snippetów.
// Upewnij się, że ServiceInProgressDTO ma CalculatedPrice i OrderSparePartDTO ma TotalItemCost.
// (Z Twoich snippetów wynika, że już je mają, co jest prawidłowe).