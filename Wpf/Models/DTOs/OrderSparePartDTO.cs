using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wpf.Models.DTOs
{
    public class OrderSparePartDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private string _id = Guid.NewGuid().ToString();
        public string Id
        {
            get => _id;
            set => SetField(ref _id, value);
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetField(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(TotalItemCost));
                }
            }
        }

        private SparePartsDTO? _sparePart;
        public SparePartsDTO? SparePart
        {
            get => _sparePart;
            set
            {
                if (SetField(ref _sparePart, value))
                {
                    Id = value?.Id.ToString() ?? string.Empty;
                    // Ustawiamy Price na wartość z katalogu, jeśli wybrano część
                    if (value != null)
                        Price = value.Price;
                    OnPropertyChanged(nameof(Id));
                    OnPropertyChanged(nameof(TotalItemCost));
                }
            }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (SetField(ref _price, value))
                {
                    OnPropertyChanged(nameof(TotalItemCost));
                }
            }
        }

        public decimal TotalItemCost => Price * Quantity;
    }
}
