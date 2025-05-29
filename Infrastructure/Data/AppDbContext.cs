
using Application;
using Domain.Entities;
using Domain.Entities.Auth;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>,IAppDbContext
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

    public AppDbContext(DbContextOptions options) : base(options)
    {   
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ClientConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderSparePartConfiguration());
        modelBuilder.ApplyConfiguration(new ServiceConfiguration());
        modelBuilder.ApplyConfiguration(new SparePartConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new WorkerConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}