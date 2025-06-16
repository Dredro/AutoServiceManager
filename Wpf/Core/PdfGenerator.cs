using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Wpf.Models.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace Wpf.Core
{
    class PdfGenerator
    {
        public static void GenerateInvoice(string filePath, OrderDTO order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //if(order.IsPaid==false || order.FinalizationDate == null)
            //    throw new InvalidOperationException("Nie można wygenerować faktury dla niezapłaconego zamówienia.");
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A4);

                    page.Header().Text("FAKTURA VAT\nnr "+order.Id)
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                    
                    page.Content().Column(content => {
                        content.Item().Height(30);
                        content.Item().AlignLeft().Text($"Data ukończnia usługi: {order.FinalizationDate}")
                        .FontSize(12);

                        content.Item().AlignLeft().Text($"Klient: {order.Client?.FirstName} {order.Client?.LastName}").FontSize(12);
                        if (order.Vehicle != null)
                        {
                            content.Item().AlignLeft().Text($"Pojazd: {order.Vehicle.Make} {order.Vehicle.Model} ({order.Vehicle.RegistrationNumber})").FontSize(12);
                        }
                        content.Item().Height(50);
                        content.Item().AlignCenter().Text($"Wykonane usługi").FontSize(16).FontColor(Colors.Blue.Medium);


                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Nazwa
                                columns.RelativeColumn(5); // Opis
                                columns.RelativeColumn(1); // Cena
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Usługa");
                                header.Cell().Element(CellStyle).Text("Opis");
                                header.Cell().Element(CellStyle).AlignRight().Text("Cena");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten3);
                                }
                            });

                            // Data rows
                            foreach (var servicesToDo in order.ServicesToDo)
                            {
                                table.Cell().Element(CellStyle).Text($"{servicesToDo.Service?.Name}");
                                table.Cell().Element(CellStyle).Text($"{servicesToDo.Service?.Description}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{servicesToDo.Price} zł");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                        content.Item().Height(10);
                        content.Item().AlignCenter().Text($"Łączny koszt usług: {order.ServicesToDo.Sum(s => s.Price):F2} zł").FontSize(16).FontColor(Colors.Green.Darken1);
                        content.Item().Height(50);
                        content.Item().AlignCenter().Text($"Wykorzystane części zamienne").FontSize(16).FontColor(Colors.Blue.Medium);



                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Numer katalogowy
                                columns.RelativeColumn(3); // Nazwa
                                columns.RelativeColumn(2); // Marka
                                columns.RelativeColumn(1); // Jakość
                                columns.RelativeColumn(1); // Ilość
                                columns.RelativeColumn(1); // Cena
                            });
                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Numer katalogowy");
                                header.Cell().Element(CellStyle).Text("Nazwa");
                                header.Cell().Element(CellStyle).Text("Marka");
                                header.Cell().Element(CellStyle).Text("Jakość");
                                header.Cell().Element(CellStyle).Text("Ilość");
                                header.Cell().Element(CellStyle).AlignRight().Text("Cena");
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten3);
                                }
                            });
                            // Data rows
                            foreach (var sparePart in order.SpareParts)
                            {
                                table.Cell().Element(CellStyle).Text($"{sparePart.SparePart?.CatalogNumber}");
                                table.Cell().Element(CellStyle).Text($"{sparePart.SparePart?.Name}");
                                table.Cell().Element(CellStyle).Text($"{sparePart.SparePart?.Make}");
                                table.Cell().Element(CellStyle).Text($"{sparePart.SparePart?.Quality}");
                                table.Cell().Element(CellStyle).Text($"{sparePart.Quantity}");
                                table.Cell().Element(CellStyle).AlignRight().Text($"{sparePart.SparePart?.Price * sparePart.Quantity:F2} zł");
                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            }
                        });
                        content.Item().Height(10);
                        content.Item().AlignCenter().Text($"Łączny koszt części zamiennych: {order.SpareParts.Sum(p => p.SparePart?.Price * p.Quantity):F2} zł").FontSize(16).FontColor(Colors.Green.Darken1);
                        content.Item().Height(50);
                        content.Item().AlignCenter().Text($"Łączny koszt zamówienia: {order.TotalCost:F2} zł").FontSize(18).FontColor(Colors.Green.Darken1);

                    });

                    
                    page.Footer().AlignCenter().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
                });
            });

            using var stream = File.Create(filePath);
            document.GeneratePdf(stream);
        }
    }
}
