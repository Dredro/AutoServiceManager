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
        // Rejestracja HttpClient
        services.AddHttpClient();

        // Rejestracja API Client i serwisów jako Singleton (zazwyczaj stateless, więc jeden egzemplarz wystarczy)
        services.AddSingleton<ApiClient>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<ClientService>();
        services.AddSingleton<OrderService>();
        services.AddSingleton<ServiceService>();
        services.AddSingleton<SparePartService>();
        services.AddSingleton<VehicleService>();
        services.AddSingleton<WorkerService>();
        
        // Rejestracja ViewModels
        // MainWindowViewModel jako Singleton, ponieważ zarządza całym oknem aplikacji
        services.AddSingleton<MainWindowViewModel>(); 
        
        // LoginViewModel jako Transient - nowa instancja przy każdej potrzebie logowania
        services.AddTransient<LoginViewModel>(); 
        
        // MainViewModel jako Transient - nowa instancja po udanym logowaniu
        services.AddTransient<MainViewModel>();

        // Rejestracja ViewModels dashboardów jako Transient (tworzone na żądanie)
        services.AddTransient<WorkerDashboardViewModel>();
        services.AddTransient<StorageManagerDashboardViewModel>();
        services.AddTransient<ManagerDashboardViewModel>();
        
        // Rejestracja pozostałych ViewModels jako Transient
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<OrderFormViewModel>();
        services.AddTransient<VehiclesListViewModel>();
        services.AddTransient<CreateCarFormViewModel>(); // Dodano: Rejestracja CreateCarFormViewModel
        services.AddTransient<ClientsListViewModel>(); // Dodano: Zakładam, że istnieje ClientsListViewModel
                                                       // i potrzebuje ClientService
                                                       services.AddTransient<CreateClientFormViewModel>(); 
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Rozwiązujemy MainWindow i ustawiamy jego DataContext na MainWindowViewModel
        var mainWindow = new MainWindow(); 
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindow.Show();
    }
}