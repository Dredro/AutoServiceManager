using Application.Orders.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;

public class PdfReportService
{
    public async Task GenerateCompletedOrdersReportAsync(List<OrderDto> completedOrders, string filePath)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);

                page.Header().Text("Raport Zrealizowanych Zleceń")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Opis
                        columns.RelativeColumn(1); // Cena
                    });

                    // Header row
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Opis Zlecenia");
                        header.Cell().Element(CellStyle).AlignRight().Text("Cena");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten3);
                        }
                    });

                    // Data rows
                    foreach (var order in completedOrders)
                    {
                        table.Cell().Element(CellStyle).Text(order.Description);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{order.Price} zł");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                });

                page.Footer().AlignCenter().Text($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}");
            });
        });

        using var stream = File.Create(filePath);
        document.GeneratePdf(stream);
    }
}
