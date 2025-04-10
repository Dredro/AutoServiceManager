using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class OrderSparePartConfiguration : IEntityTypeConfiguration<OrderSparePart>
{
    public void Configure(EntityTypeBuilder<OrderSparePart> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasOne(o => o.Order)
            .WithMany(o => o.SpareParts)
            .OnDelete(DeleteBehavior.Cascade); 
        builder.HasOne(o=>o.SparePart)
            .WithMany(s=>s.PartsAssignedToOrder)
            .OnDelete(DeleteBehavior.NoAction);
    }
}