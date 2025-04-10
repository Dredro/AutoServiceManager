using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.HasOne(n => n.Sender).WithMany(n => n.SentNotifications).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(n => n.Receivers).WithMany(w => w.Notifications);
    }
}