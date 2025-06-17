using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Wpf.Services;
using Wpf.ViewModel;
using Wpf.ViewModel.Manager;      
using Wpf.ViewModel.Worker;      
using Wpf.ViewModels;
using Wpf.ViewModels.StorageManager; 

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
        QuestPDF.Settings.License = LicenseType.Community;
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
        services.AddTransient<CreatePartFormViewModel>();
        services.AddTransient<EditClientFormViewModel>();
        services.AddTransient<EditServiceFormViewModel>();
        services.AddTransient<EditVehicleFormViewModel>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var mainWindow = new MainWindow(); 
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }
}