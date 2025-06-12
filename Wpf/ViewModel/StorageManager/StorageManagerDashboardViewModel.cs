using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models.DTOs;
using Wpf.Services;

namespace Wpf.ViewModels.StorageManager
{
    public class StorageManagerDashboardViewModel : INotifyPropertyChanged
    {
        private readonly SparePartService _sparePartService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? AddPartRequested;

        private ObservableCollection<SparePartsDTO> _allAvailableParts = new ObservableCollection<SparePartsDTO>();
        private ObservableCollection<SparePartsDTO> _allMissingParts = new ObservableCollection<SparePartsDTO>();
        private SparePartsDTO? _selectedAvailablePart;
        private SparePartsDTO? _selectedMissingPart;
        private string _availablePartsSearchText = "Wyszukaj część...";
        private string _missingPartsSearchText = "Wyszukaj brakującą część...";
        private bool _isLoading;

        public ICollectionView AvailablePartsView { get; private set; }
        public ICollectionView MissingPartsView { get; private set; }

        public ObservableCollection<SparePartsDTO> AllAvailableParts
        {
            get => _allAvailableParts;
            set
            {
                if (_allAvailableParts != value)
                {
                    _allAvailableParts = value;
                    OnPropertyChanged(nameof(AllAvailableParts));
                    AvailablePartsView = CollectionViewSource.GetDefaultView(_allAvailableParts);
                    AvailablePartsView.Filter = FilterAvailableParts;
                    OnPropertyChanged(nameof(AvailablePartsView));
                }
            }
        }

        public ObservableCollection<SparePartsDTO> AllMissingParts
        {
            get => _allMissingParts;
            set
            {
                if (_allMissingParts != value)
                {
                    _allMissingParts = value;
                    OnPropertyChanged(nameof(AllMissingParts));
                    MissingPartsView = CollectionViewSource.GetDefaultView(_allMissingParts);
                    MissingPartsView.Filter = FilterMissingParts;
                    OnPropertyChanged(nameof(MissingPartsView));
                }
            }
        }

        public SparePartsDTO? SelectedAvailablePart
        {
            get => _selectedAvailablePart;
            set
            {
                if (_selectedAvailablePart != value)
                {
                    _selectedAvailablePart = value;
                    OnPropertyChanged(nameof(SelectedAvailablePart));
                }
            }
        }

        public SparePartsDTO? SelectedMissingPart
        {
            get => _selectedMissingPart;
            set
            {
                if (_selectedMissingPart != value)
                {
                    _selectedMissingPart = value;
                    OnPropertyChanged(nameof(SelectedMissingPart));
                    ((RelayCommand)OrderSelectedMissingPartsCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string AvailablePartsSearchText
        {
            get => _availablePartsSearchText;
            set
            {
                if (_availablePartsSearchText != value)
                {
                    _availablePartsSearchText = value;
                    OnPropertyChanged(nameof(AvailablePartsSearchText));
                    AvailablePartsView.Refresh();
                }
            }
        }

        public string MissingPartsSearchText
        {
            get => _missingPartsSearchText;
            set
            {
                if (_missingPartsSearchText != value)
                {
                    _missingPartsSearchText = value;
                    OnPropertyChanged(nameof(MissingPartsSearchText));
                    MissingPartsView.Refresh();
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
                    ((RelayCommand)AddPartCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)OrderSelectedMissingPartsCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand AddPartCommand { get; }
        public ICommand OrderSelectedMissingPartsCommand { get; }
        public ICommand SearchTextBoxGotFocusCommand { get; }
        public ICommand SearchTextBoxLostFocusCommand { get; }

        public StorageManagerDashboardViewModel(SparePartService sparePartService)
        {
            _sparePartService = sparePartService ?? throw new ArgumentNullException(nameof(sparePartService));

            AvailablePartsView = CollectionViewSource.GetDefaultView(AllAvailableParts);
            AvailablePartsView.Filter = FilterAvailableParts;
            MissingPartsView = CollectionViewSource.GetDefaultView(AllMissingParts);
            MissingPartsView.Filter = FilterMissingParts;

            AddPartCommand = new RelayCommand(OnAddPart, CanExecuteCommands);
            OrderSelectedMissingPartsCommand = new RelayCommand(OnOrderSelectedMissingParts, CanOrderSelectedMissingParts);
            
            SearchTextBoxGotFocusCommand = new RelayCommand<string>(OnSearchTextBoxGotFocus);
            SearchTextBoxLostFocusCommand = new RelayCommand<string>(OnSearchTextBoxLostFocus);

            LoadPartsAsync();
        }

        private bool FilterAvailableParts(object item)
        {
            var part = item as SparePartsDTO;
            if (string.IsNullOrWhiteSpace(AvailablePartsSearchText) || AvailablePartsSearchText == "Wyszukaj część...")
            {
                return true;
            }
            return part != null &&
                   (part.Name?.Contains(AvailablePartsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    part.CatalogNumber?.Contains(AvailablePartsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    part.Make?.Contains(AvailablePartsSearchText, StringComparison.OrdinalIgnoreCase) == true);
        }

        private bool FilterMissingParts(object item)
        {
            var part = item as SparePartsDTO;
            if (string.IsNullOrWhiteSpace(MissingPartsSearchText) || MissingPartsSearchText == "Wyszukaj brakującą część...")
            {
                return true;
            }
            return part != null &&
                   (part.Name?.Contains(MissingPartsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    part.CatalogNumber?.Contains(MissingPartsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                    part.Make?.Contains(MissingPartsSearchText, StringComparison.OrdinalIgnoreCase) == true);
        }

        public void Refresh()
        {
           LoadPartsAsync();
        }
        private async void LoadPartsAsync()
        {
            IsLoading = true;
            try
            {
                var allParts = await _sparePartService.GetSparePartsAsync();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AllAvailableParts.Clear();
                    AllMissingParts.Clear();
                });
                

                if (allParts != null)
                {
                    foreach (var part in allParts)
                    {
                        Application.Current.Dispatcher.Invoke(() => AllAvailableParts.Add(part));
                        if (part.QuantityInStock == 0)
                        {
                            Application.Current.Dispatcher.Invoke(() => AllMissingParts.Add(part));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nie udało się załadować listy części.", "Błąd Ładowania", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas ładowania części: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnAddPart()
        {
            AddPartRequested?.Invoke();
        }

        private bool CanExecuteCommands()
        {
            return !IsLoading;
        }

        private void OnOrderSelectedMissingParts()
        {
            if (SelectedMissingPart != null)
            {
                MessageBox.Show($"Zlecono zamówienie części: {SelectedMissingPart.Name} (Numer katalogowy: {SelectedMissingPart.CatalogNumber})", "Zamówienie Części", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanOrderSelectedMissingParts()
        {
            return SelectedMissingPart != null && !IsLoading;
        }

        private void OnSearchTextBoxGotFocus(string parameter)
        {
            if (parameter == "AvailableParts")
            {
                if (AvailablePartsSearchText == "Wyszukaj część...")
                {
                    AvailablePartsSearchText = string.Empty;
                }
            }
            else if (parameter == "MissingParts")
            {
                if (MissingPartsSearchText == "Wyszukaj brakującą część...")
                {
                    MissingPartsSearchText = string.Empty;
                }
            }
        }

        private void OnSearchTextBoxLostFocus(string parameter)
        {
            if (parameter == "AvailableParts")
            {
                if (string.IsNullOrWhiteSpace(AvailablePartsSearchText))
                {
                    AvailablePartsSearchText = "Wyszukaj część...";
                }
            }
            else if (parameter == "MissingParts")
            {
                if (string.IsNullOrWhiteSpace(MissingPartsSearchText))
                {
                    MissingPartsSearchText = "Wyszukaj brakującą część...";
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}