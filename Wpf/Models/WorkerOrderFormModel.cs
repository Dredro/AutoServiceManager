using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wpf.Models.DTOs;

namespace Wpf.Models;

public class WorkerOrderFormModel : INotifyPropertyChanged
{
	private string _orderNameText = String.Empty;
    private string _searchCustomersString = String.Empty;
    private string _searchVechicleString = String.Empty;

    public string OrderNameText
    {
		get { return _orderNameText; }
        set => SetProperty(ref _orderNameText, value);
    }

    public string SearchCustomersString
    {
        get { return _searchCustomersString; }
        set => SetProperty(ref _searchCustomersString, value);
    }

    public ObservableCollection<ClientDTO>? SearchedClients { get; set; }


    public string SearchVechicleString
    {
        get { return _searchVechicleString; }
        set => SetProperty(ref _searchVechicleString, value);
    }

    public ObservableCollection<VehicleDTO>? SearchedVechicles { get; set; }




    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
