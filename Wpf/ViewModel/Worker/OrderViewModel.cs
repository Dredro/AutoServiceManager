using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Document = QuestPDF.Fluent.Document;

namespace Wpf.ViewModel.Worker
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService;

        public event Action<Guid?>? RequestOrderFormView;

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<OrderDTO> _orders = new ObservableCollection<OrderDTO>();
        private bool _isLoading;
       
        public ObservableCollection<OrderDTO> Orders
        {
            get => _orders;
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                    // Zaktualizuj stan komend zależnych od IsLoading
                    // Sprawdzenie czy komenda jest typu RelayCommand, zanim rzutujemy
                    if (AddNewOrderCommand is RelayCommand addNewCommand)
                    {
                        addNewCommand.RaiseCanExecuteChanged();
                    }
                    if (DeleteOrderCommand is RelayCommand<OrderDTO> deleteCommand)
                    {
                        deleteCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        public ICommand AddNewOrderCommand { get; }
        public ICommand ShowOrderCommand { get; }
        public ICommand EditOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public Visibility CanAdd { get; set; }

        private readonly AuthService _authService;

        public OrdersViewModel(OrderService orderService, AuthService authService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            AddNewOrderCommand = new RelayCommand(OnAddNewOrder, () => !IsLoading);
            ShowOrderCommand = new RelayCommand<OrderDTO>(OnShowOrder);
            EditOrderCommand = new RelayCommand<OrderDTO>(OnEditOrder);
            DeleteOrderCommand = new RelayCommand<OrderDTO>(async (order) => await OnDeleteOrder(order), (order) => !IsLoading);

            // Uruchomienie ładowania bez oczekiwania
            _ = LoadOrdersAsync();

            if (AuthService.CurrentRole == Models.Role.Manager) CanAdd = Visibility.Visible; else CanAdd = Visibility.Hidden;
        }

        public async Task LoadOrdersAsync()
        {
            IsLoading = true;
            try
            {
                var loadedOrders = await _orderService.GetOrdersAsync();
                Orders.Clear();
                if (loadedOrders != null)
                {
                    foreach (var order in loadedOrders)
                    {
                        Orders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania zleceń: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnAddNewOrder()
        {
            RequestOrderFormView?.Invoke(null);
        }

        private void OnShowOrder(OrderDTO? order)
        {
            if (order != null)
            {
                GenerateOrderSummaryPdf(order);
                /*MessageBox.Show($"Wyświetl szczegóły zlecenia ID: {order.Id}\n" +
                                $"Klient: {order.Client.FirstName} {order.Client?.LastName}\n" +
                                $"Pojazd: {order.Vehicle?.Make} {order.Vehicle?.Model} ({order.Vehicle?.RegistrationNumber})\n" +
                                $"Status: {order.Status}\n" +
                                $"Koszt: {order.TotalCost:C}", 
                                "Szczegóły Zlecenia", MessageBoxButton.OK, MessageBoxImage.Information);*/
            }
        }

        private void GenerateOrderSummaryPdf(OrderDTO order)
        {
            const string CompanyName = "Twoja Firma Serwisowa Sp. z o.o.";
            const string CompanyAddress = "ul. Przykładowa 123, 00-001 Miasto";
            const string CompanyNIP = "PL1234567890";
            const string CompanyPhone = "+48 123 456 789";
            const string CompanyEmail = "kontakt@twojafirma.pl";

            const decimal VatRate = 0.23m; // 23%

            decimal totalGross = order.TotalCost;
            decimal totalNet = Math.Round(totalGross / (1 + VatRate), 2);
            decimal totalVatAmount = totalGross - totalNet;

            string fileName = $"Podsumowanie_Zlecenia_{order.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .PaddingBottom(20)
                        .Text("Podsumowanie Zlecenia / Pseudo Faktura VAT")
                        .SemiBold().FontSize(18).AlignCenter();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Sprzedawca:").Bold();
                                    col.Item().Text(CompanyName);
                                    col.Item().Text(CompanyAddress);
                                    col.Item().Text($"NIP: {CompanyNIP}");
                                    col.Item().Text($"Tel: {CompanyPhone}");
                                    col.Item().Text($"Email: {CompanyEmail}");
                                });

                                row.RelativeItem().AlignRight().Column(col =>
                                {
                                    col.Item().Text($"Data wystawienia: {DateTime.Now:dd.MM.yyyy}");
                                    col.Item().Text($"Numer dokumentu: ZL/{order.Id}/{DateTime.Now.Year}");
                                });
                            });

                            // --- Sekcja Nabywca ---
                            column.Item().Text("Nabywca:").Bold();
                            column.Item().Text($"{order.Client.FirstName} {order.Client.LastName}");
                            column.Item().Text($"{order.Client.Email}");
                            column.Item().Text($"{order.Client.PhoneNumber}");

                            // --- Sekcja Dane Pojazdu ---
                            column.Item().Text("Dane Pojazdu:").Bold();
                            column.Item().Text($"Marka: {order.Vehicle?.Make ?? "N/A"}");
                            column.Item().Text($"Model: {order.Vehicle?.Model ?? "N/A"}");
                            column.Item().Text($"Numer rejestracyjny: {order.Vehicle?.RegistrationNumber ?? "N/A"}");
                            // FIX: 'col' was out of scope here. Use 'column.Item()' as it's directly in the main column.
                            column.Item().Text($"VIN: {order.Vehicle?.Vin ?? "N/A"}");

                            // --- Sekcja Szczegóły Zlecenia ---
                            column.Item().Text("Szczegóły Zlecenia:").Bold();
                            column.Item().Text($"ID Zlecenia: {order.Id}");
                            column.Item().Text($"Status: {order.Status}");
                            column.Item().Text($"Data zakończenia: {order.FinalizationDate:dd.MM.yyyy HH:mm}");


                            // --- Tabela usług/kosztów ---
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Opis
                                    columns.ConstantColumn(80); // Koszt Netto
                                    columns.ConstantColumn(50); // VAT (%)
                                    columns.ConstantColumn(80); // VAT Kwota
                                    columns.ConstantColumn(90); // Koszt Brutto
                                });

                                table.Header(header =>
                                {
                                    header.Cell().PaddingBottom(5).Text("Opis").Bold();
                                    header.Cell().PaddingBottom(5).AlignRight().Text("Koszt Netto").Bold();
                                    header.Cell().PaddingBottom(5).AlignRight().Text("VAT (%)").Bold();
                                    header.Cell().PaddingBottom(5).AlignRight().Text("VAT Kwota").Bold();
                                    header.Cell().PaddingBottom(5).AlignRight().Text("Koszt Brutto").Bold();
                                });

                                // Pojedyncza linia dla "Usługi serwisowe"
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5)
                                    .Text("Usługi serwisowe / Podsumowanie zlecenia").FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5)
                                    .AlignRight().Text($"{totalNet:C2}").FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5)
                                    .AlignRight().Text($"{VatRate * 100}%").FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5)
                                    .AlignRight().Text($"{totalVatAmount:C2}").FontSize(9);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5)
                                    .AlignRight().Text($"{totalGross:C2}").FontSize(9);

                                // Suma końcowa
                                table.Footer(footer =>
                                {
                                    footer.Cell().ColumnSpan(3).PaddingTop(10).AlignRight().Text("RAZEM NETTO:")
                                        .SemiBold();
                                    footer.Cell().PaddingTop(10).AlignRight().Text($"{totalNet:C2}")
                                        .SemiBold(); // This one doesn't have ColumnSpan, so it's fine.
                                    footer.Cell();
                                    footer.Cell();

                                    footer.Cell().ColumnSpan(3).AlignRight().Text("RAZEM VAT:").SemiBold();
                                    footer.Cell();
                                    footer.Cell();
                                    footer.Cell().AlignRight().Text($"{totalVatAmount:C2}").SemiBold();
                                    footer.Cell();

                                    footer.Cell().ColumnSpan(4).PaddingTop(5).AlignRight()
                                        .Text("KWOTA DO ZAPŁATY (BRUTTO):").Bold().FontSize(12);
                                    footer.Cell().PaddingTop(5).AlignRight().Text($"{totalGross:C2}").Bold()
                                        .FontSize(12);
                                });
                            });

                            column.Item().PaddingTop(20).Text("Sposób płatności: Przelew bankowy").FontSize(9);
                            column.Item().Text("Termin płatności: 7 dni od daty wystawienia").FontSize(9);
                            column.Item().Text("Dziękujemy za skorzystanie z naszych usług!").FontSize(9).Italic();

                        });

                    page.Footer()
                        .AlignRight()
                        .Text(x =>
                        {
                            x.Span("Strona ");
                            x.CurrentPageNumber();
                            x.Span(" z ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf(filePath);

            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się otworzyć pliku PDF: {ex.Message}", "Błąd", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            MessageBox.Show($"Podsumowanie zlecenia zostało wygenerowane w pliku:\n{filePath}", "Generowanie PDF",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void OnEditOrder(OrderDTO? order)
        {
            if (order != null)
            {
                RequestOrderFormView?.Invoke(order.Id);
            }
        }

        private async Task OnDeleteOrder(OrderDTO? order)
        {
            if (order == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć zlecenie ID: {order.Id} (Klient: {order.Client?.FirstName} {order.Client?.LastName}, Pojazd: {order.Vehicle?.RegistrationNumber})?",
                "Potwierdź usunięcie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                IsLoading = true;
                try
                {
                    // Tutaj powinna być rzeczywista logika usuwania w serwisie:
                    // await _orderService.DeleteOrderAsync(order.Id);

                    // Symulacja usunięcia:
                    MessageBox.Show($"Funkcjonalność usuwania zlecenia {order.Id} niezaimplementowana w serwisie. Symulacja usunięcia.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                    Orders.Remove(order);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas usuwania zlecenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        protected void OnPropertyChanged(string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}