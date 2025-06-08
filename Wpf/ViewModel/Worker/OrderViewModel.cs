using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;
using Wpf.Views.Worker;

namespace Wpf.ViewModel.Worker;

public class OrdersViewModel : BaseViewModel
{
    private ObservableCollection<OrderDTO> _orders = new();
    private OrderDTO _selectedOrder;

    // Tymczasowe dane dla ComboBoxów w dialogach
    public List<ServiceDTO> AvailableServices { get; set; } = new();
    public List<SparePartsDTO> AvailableParts { get; set; } = new();

    public OrdersViewModel()
    {
        // Inicjalizacja komend
        AddNewOrderCommand = new RelayCommand(AddNewOrder);
        ShowOrderCommand = new RelayCommand<OrderDTO>(ShowOrder);
        EditOrderCommand = new RelayCommand<OrderDTO>(EditOrder, CanEditOrder);
        DeleteOrderCommand = new RelayCommand<OrderDTO>(DeleteOrder);

        // Ładowanie przykładowych danych
        LoadSampleData();
        InitializeSampleServicesAndParts();
    }

    public ObservableCollection<OrderDTO> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            OnPropertyChanged();
        }
    }

    public OrderDTO SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddNewOrderCommand { get; }
    public ICommand ShowOrderCommand { get; }
    public ICommand EditOrderCommand { get; }
    public ICommand DeleteOrderCommand { get; }

    private void LoadSampleData()
    {
        var client = new ClientDTO
        {
            Id = Guid.NewGuid(),
            PersonalInfo = new PersonalInfo
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@example.com",
                PhoneNumber = "123456789"
            }
        };

        var vehicle = new VehicleDTO
        {
            Id = Guid.NewGuid(),
            Make = "Toyota",
            Model = "Corolla",
            Vin = "JT2BF22K1W0123456",
            RegistrationNumber = "WA12345",
            YearOfProduction = 2018,
            Client = client
        };

        var AvailableServices = new List<ServiceDTO>
        {
            new ServiceDTO
            {
                Id = Guid.NewGuid(),
                Name = "Oil change",
                Description = "Standard engine oil change",
                MinimalPrice = 100,
                MaximalPrice = 200
            },
            new ServiceDTO
            {
                Id = Guid.NewGuid(),
                Name = "Brake pad replacement",
                Description = "Front brake pad replacement",
                MinimalPrice = 200,
                MaximalPrice = 350
            }
        };

        var AvailableParts = new List<SparePartsDTO>
        {
            new SparePartsDTO
            {
                Id = Guid.NewGuid(),
                CatalogNumber = "OIL-5W30-1L",
                Name = "Engine oil 5W30",
                Make = "Castrol",
                Quality = 'A',
                QuantityInStock = 10,
                Price = 50,
                Category = PartCategory.Consumables
            }
        };

        var sampleOrders = new List<OrderDTO>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Client = client,
                Vehicle = vehicle,
                IsPaid = false,
                ServicesToDo = new List<ServiceInProgressDTO>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Service = AvailableServices[0],
                        Price = 120,
                        ServiceStatus = ServiceStatus.PendingForStart
                    }
                },
                SpareParts = new List<OrderSparePartDTO>
                {
                    new()
                    {
                        SparePart = AvailableParts[0],
                        Quantity = 1
                    }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Client = client,
                Vehicle = vehicle,
                IsPaid = true,
                FinalizationDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                ServicesToDo = new List<ServiceInProgressDTO>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Service = AvailableServices[1],
                        Price = 250,
                        ServiceStatus = ServiceStatus.Completed,
                        StartDate = DateTime.Now.AddDays(-10),
                        EndDate = DateTime.Now.AddDays(-7)
                    }
                }
            }
        };

        Orders = new ObservableCollection<OrderDTO>(sampleOrders);
    }

    private void InitializeSampleServicesAndParts()
    {
        AvailableServices.AddRange(new[]
        {
                new ServiceDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Wymiana oleju",
                    MinimalPrice = 100,
                    MaximalPrice = 150
                },
                new ServiceDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Przegląd okresowy",
                    MinimalPrice = 200,
                    MaximalPrice = 300
                }
            });

        AvailableParts.AddRange(new[]
        {
                new SparePartsDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Olej silnikowy",
                    CatalogNumber = "OIL-5W30",
                    Make = "Castrol",
                    Price = 80,
                    QuantityInStock = 10,
                    Category = PartCategory.Consumables
                },
                new SparePartsDTO
                {
                    Id = Guid.NewGuid(),
                    Name = "Filtr powietrza",
                    CatalogNumber = "AIRF-123",
                    Make = "Mann",
                    Price = 45,
                    QuantityInStock = 15,
                    Category = PartCategory.Consumables
                }
            });
    }

    private void AddNewOrder()
    {
        var dialogVm = new OrderFormViewModel();
        var dialog = new OrderFormView
        {
            DataContext = dialogVm
        };

        var window = new Window
        {
            Title = "Dodaj nowe zlecenie",
            Content = dialog,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (window.ShowDialog() == true)
        {
            var newOrder = dialogVm.Model.Order;
            Orders.Add(newOrder);
        }
    }

    private void ShowOrder(OrderDTO order)
    {
        if (order == null) return;

        var dialogVm = new OrderDetailsDialogViewModel(order);
        var dialog = new OrderDetailsDialogView
        {
            DataContext = dialogVm
        };
        dialog.ShowDialog();
    }

    private void EditOrder(OrderDTO order)
    {
        if (order == null) return;

        var dialogVm = new OrderFormViewModel(order);
        var dialog = new OrderFormView
        {
            DataContext = dialogVm
        };

        var window = new Window
        {
            Title = "Edytuj zlecenie",
            Content = dialog,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (window.ShowDialog() == true)
        {
            var index = Orders.IndexOf(order);
            if (index >= 0)
            {
                // Aktualizujemy właściwości istniejącego zlecenia
                var updatedOrder = dialogVm.Model.Order;
                order.Client = updatedOrder.Client;
                order.Vehicle = updatedOrder.Vehicle;
                order.IsPaid = updatedOrder.IsPaid;
                order.FinalizationDate = updatedOrder.FinalizationDate;
                order.ServicesToDo = updatedOrder.ServicesToDo;
                order.SpareParts = updatedOrder.SpareParts;

                OnPropertyChanged(nameof(Orders));
            }
        }
    }

    private bool CanEditOrder(OrderDTO order)
    {
        return order != null && order.Status != "Zakończone";
    }
    
    private void DeleteOrder(OrderDTO order)
    {
        if (order == null) return;

        var result = MessageBox.Show($"Czy na pewno chcesz usunąć zlecenie #{order.Id}?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo);

        if (result == MessageBoxResult.Yes)
        {
            Orders.Remove(order);
        }
    }
}