using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wpf.Models.DTOs;

namespace Wpf.Models
{
    public class StorageManagerDashboardModel : INotifyPropertyChanged
    {
        private string _availablePartsSearchText = string.Empty;
        private string _missingPartsSearchText = string.Empty;

        
        public ObservableCollection<SparePartsDTO> AvailableParts { get; set; } = new();
        public ObservableCollection<SparePartsDTO> MissingParts { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
