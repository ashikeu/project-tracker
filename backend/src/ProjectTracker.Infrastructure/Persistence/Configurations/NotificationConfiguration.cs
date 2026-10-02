using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(40);
        builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Body).HasMaxLength(2000).IsRequired();
        builder.Property(n => n.RelatedEntityType).HasMaxLength(40);
        builder.Property(n => n.RowVersion).IsRowVersion();
        // "My unread notifications, newest first" is the hot query.
        builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });

        builder.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.NotificationType).HasConversion<string>().HasMaxLength(40);
        builder.Property(p => p.RowVersion).IsRowVersion();
        builder.HasIndex(p => new { p.UserId, p.NotificationType }).IsUnique();

        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
