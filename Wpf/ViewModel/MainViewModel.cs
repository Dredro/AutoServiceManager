using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Views.Worker;

namespace Wpf.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentView)));
            }
        }

        public ICommand SwitchViewCommand { get; }

        public MainViewModel()
        {
            InitializeSampleOrders();

            CurrentView = new DashboardView { DataContext = this };
            SwitchViewCommand = new RelayCommand<string>(OnSwitchView);
        }

        private void OnSwitchView(string viewName)
        {
            switch (viewName)
            {
                case "Dashboard":
                    CurrentView = new DashboardView { DataContext = this };
                    break;
                case "Orders":
                    CurrentView = new OrdersView { DataContext = this };
                    break;
                case "Parts":
                    CurrentView = new OrderFormView { DataContext = this };
                    break;
            }
        }

        #region TO REMOVE

        public string Title { get; set; }
        public Order OrderS { get; set; }
        public ObservableCollection<User> AvailableMechanics { get; set; }
        public ObservableCollection<Service> AvailableServices { get; set; }
        public ObservableCollection<ReplacementPart> AvailableParts { get; set; }

        private void test()
        {
            var customers = new ObservableCollection<Customer>
        {
            new Customer { CustomerId = 1, Name = "Jan Kowalski", Email = "jan.kowalski@example.com", Phone = "123456789" },
            new Customer { CustomerId = 2, Name = "Anna Nowak", Email = "anna.nowak@example.com", Phone = "987654321" },
            new Customer { CustomerId = 3, Name = "Piotr Wiśniewski", Email = "piotr.wisniewski@example.com", Phone = "555123456" }
        };

            var vehicles = new ObservableCollection<Vehicle>
        {
            new Vehicle { Vin = "VIN123456789", Model = "Toyota Corolla", Year = 2018, PlateNumber = "WA12345", CustomerId = 1 },
            new Vehicle { Vin = "VIN987654321", Model = "Volkswagen Golf", Year = 2020, PlateNumber = "WA67890", CustomerId = 2 },
            new Vehicle { Vin = "VIN456789123", Model = "Audi A4", Year = 2019, PlateNumber = "PO11223", CustomerId = 3 }
        };

            var mechanics = new ObservableCollection<User>
        {
            new User { UserId = 1, Username = "mechanik1", Name = "Adam Nowak", Email = "adam.nowak@warsztat.com", Role = "Mechanic" },
            new User { UserId = 2, Username = "mechanik2", Name = "Marek Kowalski", Email = "marek.kowalski@warsztat.com", Role = "Mechanic" }
        };

            var services = new ObservableCollection<Service>
        {
            new Service { ServiceId = 1, ServiceName = "Wymiana oleju i filtra", Description = "Kompleksowa wymiana oleju silnikowego i filtra oleju", Price = 199.99m, Category = "Przegląd okresowy" },
            new Service { ServiceId = 2, ServiceName = "Wymiana klocków hamulcowych", Description = "Wymiana klocków hamulcowych przednich", Price = 349.00m, Category = "Układ hamulcowy" },
            new Service { ServiceId = 3, ServiceName = "Diagnostyka komputerowa", Description = "Podłączenie skanera diagnostycznego", Price = 150.00m, Category = "Diagnostyka" }
        };

            var parts = new ObservableCollection<ReplacementPart>
        {
            new ReplacementPart { PartId = 1, PartName = "Olej silnikowy 5W30", Supplier = "Castrol", SkuCode = "OL-CAST-5W30", StockCount = 25, UnitPrice = 45.99m, WarrantyPeriod = "24 miesiące" },
            new ReplacementPart { PartId = 2, PartName = "Filtr oleju", Supplier = "Mann-Filter", SkuCode = "FIL-MANN-123", StockCount = 18, UnitPrice = 35.50m, WarrantyPeriod = "12 miesięcy" },
            new ReplacementPart { PartId = 3, PartName = "Klocki hamulcowe przód", Supplier = "Brembo", SkuCode = "KL-BREM-456", StockCount = 10, UnitPrice = 220.00m, WarrantyPeriod = "36 miesięcy" }
        };

            OrderS = new Order
            {
                OrderId = 1,
                CreatedOn = DateTime.Now.AddDays(-3),
                StartedOn = DateTime.Now.AddDays(-2),
                Status = OrderStatus.InProgress,
                CustomerId = 1,
                Customer = customers[0],
                Vin = vehicles[0].Vin,
                Vehicle = vehicles[0],
                MechanicId = 1,
                AssignedMechanic = mechanics[0],
                Services = new List<OrderService>
                {
                    new OrderService { Service = services[0], OrderId = 1, ServiceId = 1 },
                    new OrderService { Service = services[2], OrderId = 1, ServiceId = 3 }
                },
                ReplacementParts = new List<OrderPart>
                {
                    new OrderPart {  OrderId = 1, PartId = 1, Quantity = 1, Note = "Olej syntetyczny" },
                    new OrderPart {  OrderId = 1, PartId = 2, Quantity = 1, Note = "Oryginalny filtr" }
                },
                TotalCost = 199.99m + 150.00m + 45.99m + 35.50m,
                DescriptionOfWork = "Przegląd okresowy z diagnostyką",
                Remarks = "Klient zgłasza lekkie stuki z przodu"
            };

            AvailableMechanics = mechanics;
            AvailableServices = services;
            AvailableParts = parts;
        }

        

        public decimal ServicesTotalCost => OrderS.Services.Sum(s => s.Service?.Price ?? 0);
        public decimal PartsTotalCost => AvailableParts.Sum(p => (p?.UnitPrice ?? 0));

        public enum OrderStatus { New, Accecped, InProgress, Completed, Canceled }

        public enum RoleType { Mechanic = 1, WarehouseWorker = 2, Manager = 3, Admin = 4 }

        public static (ObservableCollection<Order> Orders,
                  ObservableCollection<Customer> Customers,
                  ObservableCollection<Vehicle> Vehicles,
                  ObservableCollection<User> Mechanics,
                  ObservableCollection<Service> Services,
                  ObservableCollection<ReplacementPart> Parts) GenerateAllSampleData()
        {
            // 1. Generowanie podstawowych danych
            var customers = new ObservableCollection<Customer>
        {
            new Customer { CustomerId = 1, Name = "Jan Kowalski", Email = "jan.kowalski@example.com", Phone = "123456789" },
            new Customer { CustomerId = 2, Name = "Anna Nowak", Email = "anna.nowak@example.com", Phone = "987654321" },
            new Customer { CustomerId = 3, Name = "Piotr Wiśniewski", Email = "piotr.wisniewski@example.com", Phone = "555123456" }
        };

            var vehicles = new ObservableCollection<Vehicle>
        {
            new Vehicle { Vin = "VIN123456789", Model = "Toyota Corolla", Year = 2018, PlateNumber = "WA12345", CustomerId = 1 },
            new Vehicle { Vin = "VIN987654321", Model = "Volkswagen Golf", Year = 2020, PlateNumber = "WA67890", CustomerId = 2 },
            new Vehicle { Vin = "VIN456789123", Model = "Audi A4", Year = 2019, PlateNumber = "PO11223", CustomerId = 3 }
        };

            var mechanics = new ObservableCollection<User>
        {
            new User { UserId = 1, Username = "mechanik1", Name = "Adam Nowak", Email = "adam.nowak@warsztat.com", Role = "Mechanic" },
            new User { UserId = 2, Username = "mechanik2", Name = "Marek Kowalski", Email = "marek.kowalski@warsztat.com", Role = "Mechanic" }
        };

            var services = new ObservableCollection<Service>
        {
            new Service { ServiceId = 1, ServiceName = "Wymiana oleju i filtra", Description = "Kompleksowa wymiana oleju silnikowego i filtra oleju", Price = 199.99m, Category = "Przegląd okresowy" },
            new Service { ServiceId = 2, ServiceName = "Wymiana klocków hamulcowych", Description = "Wymiana klocków hamulcowych przednich", Price = 349.00m, Category = "Układ hamulcowy" },
            new Service { ServiceId = 3, ServiceName = "Diagnostyka komputerowa", Description = "Podłączenie skanera diagnostycznego", Price = 150.00m, Category = "Diagnostyka" }
        };

            var parts = new ObservableCollection<ReplacementPart>
        {
            new ReplacementPart { PartId = 1, PartName = "Olej silnikowy 5W30", Supplier = "Castrol", SkuCode = "OL-CAST-5W30", StockCount = 25, UnitPrice = 45.99m, WarrantyPeriod = "24 miesiące" },
            new ReplacementPart { PartId = 2, PartName = "Filtr oleju", Supplier = "Mann-Filter", SkuCode = "FIL-MANN-123", StockCount = 18, UnitPrice = 35.50m, WarrantyPeriod = "12 miesięcy" },
            new ReplacementPart { PartId = 3, PartName = "Klocki hamulcowe przód", Supplier = "Brembo", SkuCode = "KL-BREM-456", StockCount = 10, UnitPrice = 220.00m, WarrantyPeriod = "36 miesięcy" }
        };

            // 2. Ustanawianie relacji
            foreach (var vehicle in vehicles)
            {
                vehicle.Customer = customers.First(c => c.CustomerId == vehicle.CustomerId);
                vehicle.Customer.Vehicles.Add(vehicle);
            }

            // 3. Generowanie zamówień z pełnymi relacjami
            var orders = new ObservableCollection<Order>
        {
            new Order
            {
                OrderId = 1,
                CreatedOn = DateTime.Now.AddDays(-3),
                StartedOn = DateTime.Now.AddDays(-2),
                Status = OrderStatus.InProgress,
                CustomerId = 1,
                Customer = customers[0],
                Vin = vehicles[0].Vin,
                Vehicle = vehicles[0],
                MechanicId = 1,
                AssignedMechanic = mechanics[0],
                Services = new List<OrderService>
                {
                    new OrderService { Service = services[0], OrderId = 1, ServiceId = 1 },
                    new OrderService { Service = services[2], OrderId = 1, ServiceId = 3 }
                },
                ReplacementParts = new List<OrderPart>
                {
                    new OrderPart {  OrderId = 1, PartId = 1, Quantity = 1, Note = "Olej syntetyczny" },
                    new OrderPart {  OrderId = 1, PartId = 2, Quantity = 1, Note = "Oryginalny filtr" }
                },
                TotalCost = 199.99m + 150.00m + 45.99m + 35.50m,
                DescriptionOfWork = "Przegląd okresowy z diagnostyką",
                Remarks = "Klient zgłasza lekkie stuki z przodu"
            },
            new Order
            {
                OrderId = 2,
                CreatedOn = DateTime.Now.AddDays(-5),
                StartedOn = DateTime.Now.AddDays(-4),
                CompletedOn = DateTime.Now.AddDays(-1),
                Status = OrderStatus.Completed,
                CustomerId = 2,
                Customer = customers[1],
                Vin = vehicles[1].Vin,
                Vehicle = vehicles[1],
                MechanicId = 2,
                AssignedMechanic = mechanics[1],
                Services = new List<OrderService>
                {
                    new OrderService { Service = services[1], OrderId = 2, ServiceId = 2 }
                },
                ReplacementParts = new List<OrderPart>
                {
                    new OrderPart { OrderId = 2, PartId = 3, Quantity = 2, Note = "Klocki premium" }
                },
                TotalCost = 349.00m + (220.00m * 2),
                DescriptionOfWork = "Wymiana klocków hamulcowych",
                Remarks = "Hamulce piszczą przy lekkim hamowaniu"
            },
            new Order
            {
                OrderId = 3,
                CreatedOn = DateTime.Now.AddHours(-2),
                Status = OrderStatus.New,
                CustomerId = 3,
                Customer = customers[2],
                Vin = vehicles[2].Vin,
                Vehicle = vehicles[2],
                Services = new List<OrderService>
                {
                    new OrderService { Service = services[0], OrderId = 3, ServiceId = 1 }
                },
                ReplacementParts = new List<OrderPart>
                {
                    new OrderPart { OrderId = 3, PartId = 1, Quantity = 1, Note = "Olej 5W40" },
                    new OrderPart { OrderId = 3, PartId = 2, Quantity = 1, Note = "Filtr węglowy" }
                },
                TotalCost = 199.99m + 45.99m + 35.50m,
                DescriptionOfWork = "Wymiana oleju i filtra",
                Remarks = "Do zatwierdzenia przez klienta"
            }
        };

            // 4. Uzupełnianie relacji w zamówieniach
            foreach (var order in orders)
            {
                order.Customer.Orders.Add(order);
                order.Vehicle.Orders.Add(order);
                if (order.MechanicId.HasValue)
                {
                    order.AssignedMechanic.AssignedOrders.Add(order);
                }
            }

            // 5. Generowanie faktur dla zakończonych zamówień
            var invoice1 = new Invoice
            {
                InvoiceId = 1,
                OrderId = 2,
                Order = orders[1],
                IssuedByUserId = 1,
                IssuedByUser = mechanics[0],
                Subtotal = orders[1].TotalCost,
                Discount = 0,
                Total = orders[1].TotalCost,
                IsPaid = true
            };

            orders[1].InvoiceId = invoice1.InvoiceId;
            orders[1].Invoice = invoice1;

            // 6. Zwracanie wszystkich danych
            return (orders, customers, vehicles, mechanics, services, parts);
        }

        public class OrderStat
        {
            public string MechanicName { get; set; }
            public int OrdersCompleted { get; set; }
            public decimal TotalRevenueGenerated { get; set; }
        }

        public class WarehouseStat
        {
            public ReplacementPart Part { get; set; }
            public int CurrentStock { get; set; }
            public int MinStockLimit { get; set; }
            public bool IsLowStock => CurrentStock < MinStockLimit;
        }


        public class Invoice
        {
            public int InvoiceId { get; set; }

            public DateTime InvoiceDate { get; set; } = DateTime.Now;

            public decimal Subtotal { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }

            public bool IsPaid { get; set; }

            public int IssuedByUserId { get; set; }
            public User IssuedByUser { get; set; }

            public int OrderId { get; set; }
            public Order Order { get; set; }
        }


        public class OrderPart
        {
            [Key]
            public int Id { get; set; }

            public int OrderId { get; set; }
            public int PartId { get; set; }

            public int Quantity { get; set; } = 1;
            public string Note { get; set; }
        }


        public class OrderService
        {
            [Key]
            public int Id { get; set; }
            public int OrderId { get; set; }
            public int ServiceId { get; set; }

            public Service Service { get; set; }
            public Order Order { get; set; }
        }


        public class ReplacementPart
        {
            public int PartId { get; set; }

            public string PartName { get; set; }

            public string Supplier { get; set; }

            public string SkuCode { get; set; }

            public int StockCount { get; set; }

            public decimal UnitPrice { get; set; }

            public string WarrantyPeriod { get; set; }
        }


        public class Service
        {
            public int ServiceId { get; set; }

            public string ServiceName { get; set; }

            public string Description { get; set; }

            public decimal Price { get; set; }

            public string Category { get; set; }
        }


        public class User
        {
            public int UserId { get; set; }

            public string Username { get; set; }

            public string Name { get; set; }

            public string Email { get; set; }

            public string Role { get; set; }

            public string PasswordHash { get; set; }

            public List<Order> AssignedOrders { get; set; } = new List<Order>();
        }


        public class Order
        {
            public int OrderId { get; set; }

            public DateTime CreatedOn { get; set; } = DateTime.Now;
            public DateTime? StartedOn { get; set; }
            public DateTime? CompletedOn { get; set; }

            public OrderStatus Status { get; set; }

            public int CustomerId { get; set; }
            public Customer Customer { get; set; }

            public string Vin { get; set; }
            public Vehicle Vehicle { get; set; }

            public int? MechanicId { get; set; }
            public User AssignedMechanic { get; set; }

            public List<OrderService> Services { get; set; } = new List<OrderService>();
            public List<OrderPart> ReplacementParts { get; set; } = new List<OrderPart>();

            public decimal TotalCost { get; set; }

            public int? InvoiceId { get; set; }
            public Invoice Invoice { get; set; }

            public string DescriptionOfWork { get; set; }
            public string Remarks { get; set; }
        }


        public class Vehicle
        {
            public string Vin { get; set; }

            public string Model { get; set; }

            public int Year { get; set; }

            public string PlateNumber { get; set; }

            public int CustomerId { get; set; }
            public Customer Customer { get; set; }

            public List<Order> Orders { get; set; } = new List<Order>();
        }


        public class Customer
        {
            public int CustomerId { get; set; }

            public string Name { get; set; }

            public string Email { get; set; }

            public string Phone { get; set; }

            public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

            public List<Order> Orders { get; set; } = new List<Order>();
        }

        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Order> OrdersList { get; set; }
        public ObservableCollection<ReplacementPart> Parts { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<OrderStat> MechanicsStats { get; set; }
        public ObservableCollection<WarehouseStat> WarehouseStats { get; set; }
        public ObservableCollection<Service> Services { get; set; }

        public void InitializeSampleOrders()
        {
            var customer3 = new Customer { CustomerId = 3, Name = "Piotr Zieliński", Email = "piotr.zielinski@example.com", Phone = "555123456" };
            var customer4 = new Customer { CustomerId = 4, Name = "Maria Wójcik", Email = "maria.wojcik@example.com", Phone = "555654321" };
            var customer5 = new Customer { CustomerId = 5, Name = "Krzysztof Lewandowski", Email = "k.lewandowski@example.com", Phone = "777888999" };

            var vehicle3 = new Vehicle { Vin = "VIN333444555", Model = "Audi A4", Year = 2019, PlateNumber = "PO11223", CustomerId = 3 };
            var vehicle4 = new Vehicle { Vin = "VIN444555666", Model = "BMW X5", Year = 2021, PlateNumber = "GD44556", CustomerId = 4 };
            var vehicle5 = new Vehicle { Vin = "VIN555666777", Model = "Skoda Octavia", Year = 2017, PlateNumber = "KR77889", CustomerId = 5 };

            var mechanic3 = new User { UserId = 3, Name = "Tomasz Nowak", Role = "Mechanic" };
            var mechanic4 = new User { UserId = 4, Name = "Robert Malinowski", Role = "Mechanic" };

            var diagnosticService = new Service { ServiceId = 3, ServiceName = "Diagnostyka komputerowa", Price = 150.00m };
            var timingBeltService = new Service { ServiceId = 4, ServiceName = "Wymiana rozrządu", Price = 1200.00m };
            var acService = new Service { ServiceId = 5, ServiceName = "Serwis klimatyzacji", Price = 299.00m };
            var sparkPlugsService = new Service { ServiceId = 6, ServiceName = "Wymiana świec zapłonowych", Price = 180.00m };

            var airFilter = new ReplacementPart { PartId = 3, PartName = "Filtr powietrza", UnitPrice = 89.99m };
            var timingBeltKit = new ReplacementPart { PartId = 4, PartName = "Komplet rozrządu", UnitPrice = 450.00m };
            var acFilter = new ReplacementPart { PartId = 5, PartName = "Filtr kabinowy", UnitPrice = 120.00m };
            var sparkPlugs = new ReplacementPart { PartId = 6, PartName = "Świece zapłonowe", UnitPrice = 45.00m };

            var orders = new List<Order>
    {
        new Order
        {
            OrderId = 4,
            CreatedOn = DateTime.Now.AddDays(-1),
            Status = OrderStatus.Completed,
            CustomerId = 3,
            Customer = customer3,
            Vin = vehicle3.Vin,
            Vehicle = vehicle3,
            MechanicId = 3,
            AssignedMechanic = mechanic3,
            Services = new List<OrderService>
            {
                new OrderService { Service = diagnosticService, OrderId = 4, ServiceId = 3 }
            },
            TotalCost = 150.00m,
            DescriptionOfWork = "Diagnostyka silnika",
            Remarks = "Check engine - kod błędu P0172"
        },
        
        // 5. Kompleksowy serwis
        new Order
        {
            OrderId = 5,
            CreatedOn = DateTime.Now.AddDays(-7),
            StartedOn = DateTime.Now.AddDays(-6),
            Status = OrderStatus.InProgress,
            CustomerId = 4,
            Customer = customer4,
            Vin = vehicle4.Vin,
            Vehicle = vehicle4,
            MechanicId = 1,
            AssignedMechanic = new User { UserId = 1, Name = "Adam Wiśniewski", Role = "Mechanic" },
            Services = new List<OrderService>
            {
                new OrderService { Service = new Service { ServiceId = 1, ServiceName = "Wymiana oleju", Price = 199.99m }, OrderId = 5, ServiceId = 1 },
                new OrderService { Service = acService, OrderId = 5, ServiceId = 5 }
            },
            ReplacementParts = new List<OrderPart>
            {
                new OrderPart { PartId = 1, OrderId = 5, Quantity = 1 },
                new OrderPart { PartId = 5, OrderId = 5, Quantity = 1 }
            },
            TotalCost = 664.98m,
            DescriptionOfWork = "Wymiana oleju + serwis klimatyzacji",
            Remarks = "Klient zgłasza słaby nawiew"
        },

        // 6. Awaria rozrządu
        new Order
        {
            OrderId = 6,
            CreatedOn = DateTime.Now.AddDays(-10),
            StartedOn = DateTime.Now.AddDays(-9),
            CompletedOn = DateTime.Now.AddDays(-2),
            Status = OrderStatus.Completed,
            CustomerId = 5,
            Customer = customer5,
            Vin = vehicle5.Vin,
            Vehicle = vehicle5,
            MechanicId = 4,
            AssignedMechanic = mechanic4,
            Services = new List<OrderService>
            {
                new OrderService { Service = timingBeltService, OrderId = 6, ServiceId = 4 }
            },
            ReplacementParts = new List<OrderPart>
            {
                new OrderPart { PartId = 4, OrderId = 6, Quantity = 1 }
            },
            TotalCost = 1650.00m,
            DescriptionOfWork = "Wymiana paska rozrządu",
            Remarks = "Pasek zerwany - wymiana z uszczerbkiem"
        },

        // 7. Przegląd okresowy
        new Order
        {
            OrderId = 7,
            CreatedOn = DateTime.Now.AddDays(-4),
            Status = OrderStatus.New,
            CustomerId = 1,
            Customer = new Customer { CustomerId = 1, Name = "Jan Kowalski", Email = "jan.kowalski@example.com", Phone = "123456789" },
            Vin = new Vehicle { Vin = "VIN123456789", Model = "Toyota Corolla", Year = 2018, PlateNumber = "WA12345", CustomerId = 1 }.Vin,
            Vehicle = new Vehicle { Vin = "VIN123456789", Model = "Toyota Corolla", Year = 2018, PlateNumber = "WA12345", CustomerId = 1 },
            Services = new List<OrderService>
            {
                new OrderService { Service = new Service { ServiceId = 1, ServiceName = "Wymiana oleju", Price = 199.99m }, OrderId = 7, ServiceId = 1 },
                new OrderService { Service = sparkPlugsService, OrderId = 7, ServiceId = 6 }
            },
            ReplacementParts = new List<OrderPart>
            {
                new OrderPart { PartId = 1, OrderId = 7, Quantity = 1 },
                new OrderPart { PartId = 6, OrderId = 7, Quantity = 4 }
            },
            TotalCost = 424.98m,
            DescriptionOfWork = "Przegląd 60 000 km",
            Remarks = "Wymiana oleju i świec"
        },

        // 8. Serwis hamulców
        new Order
        {
            OrderId = 8,
            CreatedOn = DateTime.Now.AddDays(-2),
            StartedOn = DateTime.Now.AddDays(-1),
            Status = OrderStatus.InProgress,
            CustomerId = 2,
            Customer = new Customer { CustomerId = 2, Name = "Anna Nowak", Email = "anna.nowak@example.com", Phone = "987654321" },
            Vin = new Vehicle { Vin = "VIN987654321", Model = "Volkswagen Golf", Year = 2020, PlateNumber = "WA67890", CustomerId = 2 }.Vin,
            Vehicle = new Vehicle { Vin = "VIN987654321", Model = "Volkswagen Golf", Year = 2020, PlateNumber = "WA67890", CustomerId = 2 },
            MechanicId = 2,
            AssignedMechanic = new User { UserId = 2, Name = "Marek Kowalczyk", Role = "Mechanic" },
            Services = new List<OrderService>
            {
                new OrderService { Service = new Service { ServiceId = 2, ServiceName = "Wymiana klocków hamulcowych", Price = 349.00m }, OrderId = 8, ServiceId = 2 }
            },
            ReplacementParts = new List<OrderPart>
            {
                new OrderPart { PartId = 2, OrderId = 8, Quantity = 2 },
                new OrderPart { PartId = 3, OrderId = 8, Quantity = 1 }
            },
            TotalCost = 748.99m,
            DescriptionOfWork = "Wymiana klocków i filtru powietrza",
            Remarks = "Hamulce piszczą przy lekkim hamowaniu"
        },

        // 9. Awaryjna diagnostyka
        new Order
        {
            OrderId = 9,
            CreatedOn = DateTime.Now.AddHours(-3),
            Status = OrderStatus.New,
            CustomerId = 3,
            Customer = customer3,
            Vin = vehicle3.Vin,
            Vehicle = vehicle3,
            Services = new List<OrderService>
            {
                new OrderService { Service = diagnosticService, OrderId = 9, ServiceId = 3 }
            },
            TotalCost = 150.00m,
            DescriptionOfWork = "Auto nie odpala",
            Remarks = "Awaria po myjni - prawdopodobnie elektronika"
        },

        // 10. Kompleksowy serwis
        new Order
        {
            OrderId = 10,
            CreatedOn = DateTime.Now.AddDays(-14),
            StartedOn = DateTime.Now.AddDays(-12),
            CompletedOn = DateTime.Now.AddDays(-5),
            Status = OrderStatus.Completed,
            CustomerId = 4,
            Customer = customer4,
            Vin = vehicle4.Vin,
            Vehicle = vehicle4,
            MechanicId = 1,
            AssignedMechanic = new User { UserId = 1, Name = "Adam Wiśniewski", Role = "Mechanic" },
            Services = new List<OrderService>
            {
                new OrderService { Service = new Service { ServiceId = 1, ServiceName = "Wymiana oleju", Price = 199.99m }, OrderId = 10, ServiceId = 1 },
                new OrderService { Service = new Service { ServiceId = 2, ServiceName = "Wymiana klocków hamulcowych", Price = 349.00m }, OrderId = 10, ServiceId = 2 },
                new OrderService { Service = acService, OrderId = 10, ServiceId = 5 }
            },
            ReplacementParts = new List<OrderPart>
            {
                new OrderPart { PartId = 1, OrderId = 10, Quantity = 1 },
                new OrderPart { PartId = 2, OrderId = 10, Quantity = 2 },
                new OrderPart { PartId = 5, OrderId = 10, Quantity = 1 }
            },
            TotalCost = 1013.98m,
            DescriptionOfWork = "Kompleksowy serwis przed sezonem",
            Remarks = "Klient bardzo zadowolony z obsługi"
        },

        // 11. Anulowane zlecenie
        new Order
        {
            OrderId = 11,
            CreatedOn = DateTime.Now.AddDays(-8),
            Status = OrderStatus.Canceled,
            CustomerId = 5,
            Customer = customer5,
            Vin = vehicle5.Vin,
            Vehicle = vehicle5,
            Services = new List<OrderService>
            {
                new OrderService { Service = timingBeltService, OrderId = 11, ServiceId = 4 }
            },
            TotalCost = 1200.00m,
            DescriptionOfWork = "Wymiana rozrządu (anulowane)",
            Remarks = "Klient znalazł tańszy warsztat"
        },

        // 12. Wymiana świec i filtra
        new Order
        {
            OrderId = 12,
            CreatedOn = DateTime.Now.AddHours(-5),
            Status = OrderStatus.Completed,
            CustomerId = 1,
            Customer = new Customer { CustomerId = 1, Name = "Jan Kowalski", Email = "jan.kowalski@example.com", Phone = "123456789" },
            Vin = new Vehicle { Vin = "VIN123456789", Model = "Toyota Corolla", Year = 2018, PlateNumber = "WA12345", CustomerId = 1 }.Vin,
            Vehicle = new Vehicle { Vin = "VIN123456789", Model = "Toyota Corolla", Year = 2018, PlateNumber = "WA12345", CustomerId = 1 },
            MechanicId = 3,
            AssignedMechanic = mechanic3,
            Services = new List<OrderService>
            {
                new OrderService { Service = sparkPlugsService, OrderId = 12, ServiceId = 6 }
            },
            ReplacementParts = new List<OrderPart>
            {
                new OrderPart { PartId = 6, OrderId = 12, Quantity = 4 },
                new OrderPart { PartId = 3, OrderId = 12, Quantity = 1 }
            },
            TotalCost = 359.99m,
            DescriptionOfWork = "Wymiana świec i filtra powietrza",
            Remarks = "Silnik nierówno pracuje na biegu jałowym"
        }
    };
            Orders = new();
            OrdersList = new();
            foreach (var order in orders)
            {
                if (Orders.Count < 2) Orders.Add(order);
                OrdersList.Add(order);
            }

            Services = new ObservableCollection<Service>
            {
                new Service
                {
                    ServiceId = 1,
                    ServiceName = "Wymiana oleju i filtra",
                    Description = "Kompleksowa wymiana oleju silnikowego oraz filtra oleju",
                    Price = 199.99m,
                    Category = "Przegląd okresowy"
                },
                new Service
                {
                    ServiceId = 2,
                    ServiceName = "Wymiana klocków hamulcowych",
                    Description = "Wymiana klocków hamulcowych przednich + przegląd układu hamulcowego",
                    Price = 349.00m,
                    Category = "Układ hamulcowy"
                },
                new Service
                {
                    ServiceId = 3,
                    ServiceName = "Wymiana rozrządu",
                    Description = "Wymiana paska rozrządu z napinaczem i rolkami",
                    Price = 899.00m,
                    Category = "Silnik"
                },
                new Service
                {
                    ServiceId = 4,
                    ServiceName = "Diagnostyka komputerowa",
                    Description = "Podłączenie skanera i odczyt kodów błędów",
                    Price = 120.00m,
                    Category = "Diagnostyka"
                },
                new Service
                {
                    ServiceId = 5,
                    ServiceName = "Wymiana świec zapłonowych",
                    Description = "Wymiana kompletnych świec zapłonowych",
                    Price = 180.00m,
                    Category = "Silnik"
                },
                new Service
                {
                    ServiceId = 6,
                    ServiceName = "Wymiana płynu chłodniczego",
                    Description = "Wymiana płynu chłodniczego z przepłukaniem układu",
                    Price = 249.99m,
                    Category = "Układ chłodzenia"
                },
                new Service
                {
                    ServiceId = 7,
                    ServiceName = "Geometria kół",
                    Description = "Ustawienie geometrii kół na stanowisku 3D",
                    Price = 199.00m,
                    Category = "Zawieszenie"
                },
                new Service
                {
                    ServiceId = 8,
                    ServiceName = "Wymiana akumulatora",
                    Description = "Wymiana akumulatora z diagnostyką układu ładowania",
                    Price = 299.00m,
                    Category = "Elektryka"
                },
                new Service
                {
                    ServiceId = 9,
                    ServiceName = "Przegląd klimatyzacji",
                    Description = "Przegląd układu klimatyzacji + dezynfekcja",
                    Price = 159.00m,
                    Category = "Klimatyzacja"
                },
                new Service
                {
                    ServiceId = 10,
                    ServiceName = "Wymiana opon",
                    Description = "Sezonowa wymiana opon z wyważeniem",
                    Price = 120.00m,
                    Category = "Opony"
                }
            };
            test();
        }

        #endregion
    }
}
