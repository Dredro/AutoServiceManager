using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ClientConfiguration: IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasMany(c=>c.Vehicles).WithOne(v => v.Client).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c=>c.Orders).WithOne(o => o.Client).OnDelete(DeleteBehavior.NoAction);
    }
}