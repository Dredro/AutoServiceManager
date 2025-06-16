using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; 

using Wpf.Core; 
using Wpf.Services;
using Wpf.Models.DTOs; 

namespace Wpf.ViewModels 
{
    public class ClientsListViewModel : INotifyPropertyChanged
    {
        private readonly ClientService _clientService;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public event Action? RequestCreateClientView; 
        public event Action<Guid>? RequestShowClientView; 
        public event Action<Guid>? RequestEditClientView; 
        
        

        private ObservableCollection<ClientDTO> _allClients = new ObservableCollection<ClientDTO>(); 
        private ObservableCollection<ClientDTO> _filteredClients = new ObservableCollection<ClientDTO>(); 

        private string _searchText = string.Empty;
        private bool _isLoading;

        public ObservableCollection<ClientDTO> FilteredClients
        {
            get => _filteredClients;
            set
            {
                if (_filteredClients != value)
                {
                    _filteredClients = value;
                    OnPropertyChanged(nameof(FilteredClients));
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    FilterClients(); 
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
                    OnPropertyChanged(nameof(IsLoading));
                    
                    ((RelayCommand)LoadClientsCommand).RaiseCanExecuteChanged(); 
                    ((RelayCommand)AddNewClientCommand).RaiseCanExecuteChanged(); 
                }
            }
        }

        public ICommand LoadClientsCommand { get; }
        public ICommand AddNewClientCommand { get; }
        public ICommand ShowClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }

        public ClientsListViewModel(ClientService clientService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));

            LoadClientsCommand = new RelayCommand(async () => await LoadClientsAsync(), () => !IsLoading);
            AddNewClientCommand = new RelayCommand(OnAddNewClient, () => !IsLoading);
            ShowClientCommand = new RelayCommand<ClientDTO>(OnShowClient);
            EditClientCommand = new RelayCommand<ClientDTO>(OnEditClient);
            DeleteClientCommand = new RelayCommand<ClientDTO>(async (client) => await OnDeleteClient(client), (client) => !IsLoading);

            
            _ = LoadClientsAsync(); 
        }

        public async Task LoadClientsAsync()
        {
            IsLoading = true;
            try
            {
                var loadedClients = await _clientService.GetClientsAsync();
                _allClients.Clear(); 
                if (loadedClients != null)
                {
                    foreach (var client in loadedClients)
                    {
                        _allClients.Add(client); 
                    }
                }
                FilterClients(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania klientów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterClients()
        {
            FilteredClients.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var client in _allClients)
                {
                    FilteredClients.Add(client);
                }
            }
            else
            {
                var lowerSearchText = SearchText.ToLowerInvariant();
                var filtered = _allClients.Where(c => 
                    (c?.FirstName?.ToLowerInvariant().Contains(lowerSearchText) ?? false) ||
                    (c?.LastName?.ToLowerInvariant().Contains(lowerSearchText) ?? false) ||
                    (c?.Email?.ToLowerInvariant().Contains(lowerSearchText) ?? false) ||
                    (c?.PhoneNumber?.ToLowerInvariant().Contains(lowerSearchText) ?? false) ||
                    c.Id.ToString().ToLowerInvariant().Contains(lowerSearchText)
                ).ToList();

                foreach (var client in filtered)
                {
                    FilteredClients.Add(client);
                }
            }
        }

        private void OnAddNewClient()
        {
            RequestCreateClientView?.Invoke();
        }

        private void OnShowClient(ClientDTO? client)
        {
            if (client != null)
            {
                RequestShowClientView?.Invoke(client.Id);
            }
        }

        private void OnEditClient(ClientDTO? client)
        {
            if (client != null)
            {
                RequestEditClientView?.Invoke(client.Id);
            }
        }

        private async Task OnDeleteClient(ClientDTO? client)
        {
            if (client == null) return;

            MessageBoxResult result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć klienta {client?.FirstName+" "+client?.LastName}?",
                "Potwierdź usunięcie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await _clientService.DeleteClientAsync(client!.Id.ToString());
                MessageBox.Show($"Klient {client.FirstName} {client.LastName} został usunięty.", "Usunięto", MessageBoxButton.OK, MessageBoxImage.Information);
                _allClients.Remove(client);
                FilterClients();
                OnPropertyChanged(nameof(FilteredClients)); 
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}