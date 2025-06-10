using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wpf.Models.DTOs;

namespace Wpf.Models;

public class OrderFormModel : INotifyPropertyChanged
{
    private string _orderNameText = "Nowe zamówienie";
    private string _descriptionOfWork = string.Empty;
    private string _remarks = string.Empty;
    private decimal _servicesTotalCost;
    private decimal _partsTotalCost;

    public string OrderNameText
    {
        get => _orderNameText;
        set
        {
            _orderNameText = value;
            OnPropertyChanged();
        }
    }

    public string DescriptionOfWork
    {
        get => _descriptionOfWork;
        set
        {
            _descriptionOfWork = value;
            OnPropertyChanged();
        }
    }

    public string Remarks
    {
        get => _remarks;
        set
        {
            _remarks = value;
            OnPropertyChanged();
        }
    }

    public decimal ServicesTotalCost
    {
        get => _servicesTotalCost;
        set
        {
            _servicesTotalCost = value;
            OnPropertyChanged();
        }
    }

    public decimal PartsTotalCost
    {
        get => _partsTotalCost;
        set
        {
            _partsTotalCost = value;
            OnPropertyChanged();
        }
    }

    public OrderDTO Order { get; set; } = new OrderDTO
    {
       // Client = new ClientDTO { PersonalInfo = new PersonalInfo() }
    };

    public ObservableCollection<ClientDTO> SearchedClients { get; set; } = new();
    public ObservableCollection<VehicleDTO> SearchedVehicles { get; set; } = new();
    public ObservableCollection<ServiceDTO> AvailableServices { get; set; } = new();
    public ObservableCollection<SparePartsDTO> AvailableParts { get; set; } = new();
    public ObservableCollection<WorkerDTO> AvailableMechanics { get; set; } = new();
    public ObservableCollection<ServiceStatus> ServiceStatusList { get; set; } = new();

    public void UpdateTotalCosts()
    {
        ServicesTotalCost = Order.ServicesToDo.Sum(s => s.Price ?? 0);
        PartsTotalCost = Order.SpareParts.Sum(p => (p.SparePart?.Price ?? 0) * p.Quantity);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}