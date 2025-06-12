using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wpf.Models.DTOs;

public class OrderSparePartDTO : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Dodano: Metoda OnPropertyChanged
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Helper method for INotifyPropertyChanged (już była, ale dla kontekstu)
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName); // Ta linia wywołuje PropertyChanged, ale dla nazwy właściwości, która się zmieniła
        return true;
    }

    private string _id = Guid.NewGuid().ToString();
    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (SetField(ref _quantity, value))
            {
                OnPropertyChanged(nameof(TotalItemCost)); // Teraz to wywołanie jest poprawne
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
                Id = value?.Id.ToString() ?? string.Empty; // Pamiętaj o aktualizacji Id
                OnPropertyChanged(nameof(Id)); // Warto powiadomić o zmianie Id, jeśli jest bindowane lub istotne
                OnPropertyChanged(nameof(TotalItemCost)); // Teraz to wywołanie jest poprawne
            }
        }
    }

    public decimal TotalItemCost => (SparePart?.Price ?? 0m) * Quantity;
}