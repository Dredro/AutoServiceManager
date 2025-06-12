using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Core;
using Wpf.Models;
using Wpf.Models.DTOs; // Keep this for SparePartsDTO if needed elsewhere, but not directly for Create
using Wpf.Services;

namespace Wpf.ViewModels
{
    public class CreatePartFormViewModel : INotifyPropertyChanged
    {
        private readonly SparePartService _sparePartService;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? PartCreated;

        private string _catalogNumber = string.Empty;
        private string _name = string.Empty;
        private string _make = string.Empty;
        private char? _selectedQuality;
        private int _quantityInStock;
        private decimal _price;
        private PartCategory? _selectedCategory;
        private bool _isSaving;

        public string CatalogNumber
        {
            get => _catalogNumber;
            set
            {
                if (_catalogNumber != value)
                {
                    _catalogNumber = value;
                    OnPropertyChanged(nameof(CatalogNumber));
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Make
        {
            get => _make;
            set
            {
                if (_make != value)
                {
                    _make = value;
                    OnPropertyChanged(nameof(Make));
                }
            }
        }

        public char? SelectedQuality
        {
            get => _selectedQuality;
            set
            {
                if (_selectedQuality != value)
                {
                    _selectedQuality = value;
                    OnPropertyChanged(nameof(SelectedQuality));
                }
            }
        }

        public int QuantityInStock
        {
            get => _quantityInStock;
            set
            {
                if (_quantityInStock != value)
                {
                    _quantityInStock = value;
                    OnPropertyChanged(nameof(QuantityInStock));
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
                }
            }
        }

        public PartCategory? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (_isSaving != value)
                {
                    _isSaving = value;
                    OnPropertyChanged(nameof(IsSaving));
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public List<char> Qualities { get; } = ['P', 'S'];
        public IEnumerable<PartCategory> Categories => Enum.GetValues(typeof(PartCategory)).Cast<PartCategory>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreatePartFormViewModel(SparePartService sparePartService)
        {
            _sparePartService = sparePartService ?? throw new ArgumentNullException(nameof(sparePartService));

            SaveCommand = new RelayCommand(async () => await OnSave(), CanSave);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);

            ClearForm();
        }

        private async Task OnSave()
        {
            IsSaving = true;
            try
            {
                /*if (!CanSave())
                {
                    MessageBox.Show("Proszę wypełnić wszystkie wymagane pola poprawnymi wartościami.", "Błąd Walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }*/

                var createCommand = new CreateSparePartCommand
                {
                    CatalogNumber = CatalogNumber,
                    Name = Name,
                    Make = Make,
                    Quality = SelectedQuality.Value,
                    QuantityInStock = QuantityInStock,
                    Price = Price,
                    Category = SelectedCategory.Value
                };

                var (resultString, errorMessage) = await _sparePartService.CreateSparePartAsync(createCommand);

                if (resultString != null) // Check if the result string is not null
                {
                    MessageBox.Show("Część została pomyślnie dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    PartCreated?.Invoke();
                }
                else
                {
                    MessageBox.Show($"Błąd podczas dodawania części: {errorMessage ?? "Nieznany błąd."}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd: {ex.Message}", "Błąd Krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSave()
        {
            return !IsSaving &&
                   !string.IsNullOrWhiteSpace(CatalogNumber) &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Make) &&
                   SelectedQuality.HasValue &&
                   SelectedCategory.HasValue &&
                   QuantityInStock >= 0 &&
                   Price >= 0;
        }

        private void OnCancel()
        {
            if (MessageBox.Show("Czy na pewno chcesz anulować dodawanie części? Niezapisane zmiany zostaną utracone.", "Anuluj", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        private bool CanCancel()
        {
            return !IsSaving;
        }

        private void ClearForm()
        {
            CatalogNumber = string.Empty;
            Name = string.Empty;
            Make = string.Empty;
            SelectedQuality = null;
            QuantityInStock = 0;
            Price = 0m;
            SelectedCategory = null;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
        }
    }
}