using Domain.Entities;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application;

public interface IAppDbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceInProgress> ServiceInProgresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<OrderSparePart> OrderSpareParts { get; set; }
    public DbSet<SparePart> SpareParts { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Worker> Workers { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}