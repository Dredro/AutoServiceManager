using System.Configuration; // Prawdopodobnie niepotrzebne, jeśli nie korzystasz z App.config
using System.Data; // Prawdopodobnie niepotrzebne
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Services;
using Wpf.ViewModel;
using Wpf.ViewModel.Manager;      // Dodane dla ManagerDashboardViewModel
using Wpf.ViewModel.StorageManager; // Dodane dla StorageManagerDashboardViewModel
using Wpf.ViewModel.Worker;       // Dodane dla WorkerDashboardViewModel
using Wpf.ViewModels;             // Dodane dla VehiclesListViewModel, CreateCarFormViewModel, ClientsListViewModel

namespace Wpf;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddSingleton<ApiClient>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<ClientService>();
        services.AddSingleton<OrderService>();
        services.AddSingleton<ServiceService>();
        services.AddSingleton<SparePartService>();
        services.AddSingleton<VehicleService>();
        services.AddSingleton<WorkerService>();
        
        services.AddSingleton<MainWindowViewModel>(); 
        
        services.AddTransient<LoginViewModel>(); 
        
        services.AddTransient<MainViewModel>();

        services.AddTransient<WorkerDashboardViewModel>();
        services.AddTransient<StorageManagerDashboardViewModel>();
        services.AddTransient<ManagerDashboardViewModel>();
        
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<OrderFormViewModel>();
        services.AddTransient<VehiclesListViewModel>();
        services.AddTransient<CreateCarFormViewModel>(); 
        services.AddTransient<ClientsListViewModel>(); 
        services.AddTransient<CreateClientFormViewModel>();
        services.AddTransient<ServicesListViewModel>();
        services.AddTransient<CreateServiceFormViewModel>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var mainWindow = new MainWindow(); 
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }
}