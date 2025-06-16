using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;
using System; // Dodano dla Guid i ArgumentNullException
using System.Threading.Tasks; // Dodano dla Task

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

        public OrdersViewModel(OrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            AddNewOrderCommand = new RelayCommand(OnAddNewOrder, () => !IsLoading);
            ShowOrderCommand = new RelayCommand<OrderDTO>(OnShowOrder);
            EditOrderCommand = new RelayCommand<OrderDTO>(OnEditOrder);
            DeleteOrderCommand = new RelayCommand<OrderDTO>(async (order) => await OnDeleteOrder(order), (order) => !IsLoading);

            // Uruchomienie ładowania bez oczekiwania
            _ = LoadOrdersAsync();
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
                MessageBox.Show($"Wyświetl szczegóły zlecenia ID: {order.Id}\n" +
                                $"Klient: {order.Client.FirstName} {order.Client?.LastName}\n" +
                                $"Pojazd: {order.Vehicle?.Make} {order.Vehicle?.Model} ({order.Vehicle?.RegistrationNumber})\n" +
                                $"Status: {order.Status}\n" +
                                // Używamy teraz nowej właściwości TotalCost z OrderDTO
                                $"Koszt: {order.TotalCost:C}", // Formatowanie jako waluta
                                "Szczegóły Zlecenia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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