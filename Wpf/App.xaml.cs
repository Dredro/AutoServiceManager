using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Services;
using Wpf.ViewModel;
using Wpf.ViewModel.Worker;

namespace Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
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
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<OrderFormViewModel>();
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<WorkerDashboardViewModel>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        var mainWindow = new MainWindow(); 
        mainWindow.DataContext = _serviceProvider.GetService<MainWindowViewModel>();
        mainWindow.Show();
      
    }
}