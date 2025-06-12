using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wpf.Models.DTOs;

public enum ServiceStatus
{
    PendingForStart,
    PendingForParts,
    InProgress,
    Completed,
}

public class ServiceInProgressDTO : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public Guid Id { get; set; } // This is probably the ID of the ServiceInProgress entry itself, not the Service type.

    // NEW: ServiceId property
    private Guid _serviceId;
    public Guid ServiceId
    {
        get => _serviceId;
        set => SetField(ref _serviceId, value); 
    }
    private decimal? _price;
    public decimal? Price
    {
        get => _price;
        set
        {
            if (SetField(ref _price, value))
            {
                OnPropertyChanged(nameof(CalculatedPrice));
            }
        }
    }

    private DateTime? _startDate;
    public DateTime? StartDate
    {
        get => _startDate;
        set => SetField(ref _startDate, value);
    }

    private DateTime? _endDate;
    public DateTime? EndDate
    {
        get => _endDate;
        set => SetField(ref _endDate, value);
    }

    private ServiceStatus _serviceStatus;
    public ServiceStatus ServiceStatus
    {
        get => _serviceStatus;
        set => SetField(ref _serviceStatus, value);
    }

    private ServiceDTO? _service;
    public ServiceDTO? Service
    {
        get => _service;
        set
        {
            if (SetField(ref _service, value))
            {
                ServiceId = value?.Id ?? Guid.Empty; 
                OnPropertyChanged(nameof(CalculatedPrice));
                OnPropertyChanged(nameof(ServiceId));
            }
        }
    }

    public decimal CalculatedPrice => Price ?? Service?.MaximalPrice ?? 0m;

    public OrderDTO? Order { get; set; }
    public ICollection<WorkerDTO> Workers { get; set; } = [];
    public WorkerDTO? AssignedWorker { get; set; }
}