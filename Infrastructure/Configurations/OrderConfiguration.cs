using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(o => o.ServicesToDo)
            .WithOne(s => s.Order)
            .OnDelete(DeleteBehavior.Cascade);
        //builder.HasMany(o=>o.SpareParts).WithOne(s => s.Order).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(o=>o.Vehicle).WithMany(v=>v.Orders).OnDelete(DeleteBehavior.NoAction);
    }
}