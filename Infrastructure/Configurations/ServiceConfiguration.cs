using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(s=>s.ServicesInProgress).WithOne(s=>s.Service).OnDelete(DeleteBehavior.Cascade);
    }
}