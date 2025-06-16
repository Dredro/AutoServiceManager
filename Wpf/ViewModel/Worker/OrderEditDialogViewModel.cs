/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;

namespace Wpf.ViewModel.Worker;

public class OrderEditDialogViewModel : BaseViewModel
{
    private OrderDTO _order;
    private bool _isEditMode;

    public List<ServiceDTO> AvailableServices { get; set; }
    public List<SparePartsDTO> AvailableParts { get; set; }

    public OrderEditDialogViewModel()
    {
        Order = new OrderDTO
        {
            Id = Guid.NewGuid(),
            Client = new ClientDTO(),
            Vehicle = new VehicleDTO(),
            ServicesToDo = new List<ServiceInProgressDTO>(),
            SpareParts = new List<OrderSparePartDTO>()
        };

        InitializeCommands();
    }

    public OrderEditDialogViewModel(OrderDTO order)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        InitializeCommands();
    }

    public string Title { get; set; }

    public OrderDTO Order
    {
        get => _order;
        set
        {
            _order = value;
            OnPropertyChanged();
        }
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            _isEditMode = value;
            OnPropertyChanged();
        }
    }

    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand AddServiceCommand { get; private set; }
    public ICommand RemoveServiceCommand { get; private set; }
    public ICommand AddPartCommand { get; private set; }
    public ICommand RemovePartCommand { get; private set; }

    private void InitializeCommands()
    {
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
        AddServiceCommand = new RelayCommand(AddService);
        RemoveServiceCommand = new RelayCommand<ServiceInProgressDTO>(RemoveService);
        AddPartCommand = new RelayCommand(AddPart);
        RemovePartCommand = new RelayCommand<OrderSparePartDTO>(RemovePart);
    }

    private void Save()
    {
        OnRequestClose(true);
    }

    private void Cancel()
    {
        OnRequestClose(false);
    }

    private void AddService()
    {
        Order.ServicesToDo.Add(new ServiceInProgressDTO
        {
            Id = Guid.NewGuid(),
            Service = new ServiceDTO(),
            ServiceStatus = ServiceStatus.PendingForStart
        });
    }

    private void RemoveService(ServiceInProgressDTO service)
    {
        if (service != null)
        {
            Order.ServicesToDo.Remove(service);
        }
    }

    private void AddPart()
    {
        Order.SpareParts.Add(new OrderSparePartDTO
        {
            SparePart = new SparePartsDTO()
        });
    }

    private void RemovePart(OrderSparePartDTO part)
    {
        if (part != null)
        {
            Order.SpareParts.Remove(part);
        }
    }
}
*/
