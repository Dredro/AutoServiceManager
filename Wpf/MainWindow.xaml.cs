using System.Configuration;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Wpf.Services;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.ViewModel;
using Wpf.Views;

namespace Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //string filepath = "I:\\plik.pdf";
            //OrderDTO order = new OrderDTO
            //{
            //    Id = Guid.NewGuid(),
            //    FinalizationDate = DateOnly.FromDateTime(DateTime.Now),
            //    IsPaid = true,
            //    Client = new ClientDTO { 
            //        Id=Guid.NewGuid(),
            //        PersonalInfo = new PersonalInfo
            //        {
            //        FirstName = "Jan",
            //        LastName = "Kowalski",
            //    }, },
            //    Vehicle = new VehicleDTO{Id=Guid.NewGuid(),
            //        Make = "Toyota",
            //        Model = "Corolla",
            //        Vin = "1HGBH41JXMN109186",
            //        RegistrationNumber = "ABC1234",
            //        RegistrationDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-2)),
            //        YearOfProduction = 2021,
            //        EngineCode = "2ZR-FE",
            //        EngineDisplacement = 1800,
            //        Power = 132.0m,
            //        Type = VehicleType.Car,

            //    },
            //    ServicesToDo = new List<ServiceInProgressDTO>
            //    {
            //        new ServiceInProgressDTO
            //        {
            //            Id = Guid.NewGuid(),
            //            StartDate = DateTime.Now.AddDays(-2),
            //            EndDate = DateTime.Now.AddDays(-1),
            //            Service = new ServiceDTO
            //            {
            //                Id = Guid.NewGuid(),
            //                Name = "Wymiana oleju",
            //                Description = "Wymiana oleju w silniku.",
                           
            //            },
            //            ServiceStatus = ServiceStatus.Completed,
            //            Price = 100.00m
            //        },
            //    },
            //};

            //order.SpareParts.Add(new OrderSparePartDTO
            //{
            //    SparePart = new SparePartsDTO
            //    {
            //        CatalogNumber = "MAX09",
            //        Id = Guid.NewGuid(),
            //        Name = "Filtr oleju",
            //        Price = 20.00m,
            //        Make = "Hilfo",
            //        Quality = 'O',
            //        QuantityInStock = 50,
            //        Category = PartCategory.Powertrain,
            //    },
            //    Quantity = 1
            //});

            //PdfGenerator.GenerateInvoice(filepath,order);
        }
    }
}