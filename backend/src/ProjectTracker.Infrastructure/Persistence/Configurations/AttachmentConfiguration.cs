using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.EntityType).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.FileName).HasMaxLength(300).IsRequired();
        builder.Property(a => a.OriginalFileName).HasMaxLength(300).IsRequired();
        builder.Property(a => a.ContentType).HasMaxLength(150).IsRequired();
        builder.Property(a => a.StoragePath).HasMaxLength(500).IsRequired();
        builder.Property(a => a.RowVersion).IsRowVersion();
        builder.HasIndex(a => a.OrganizationId);
        // Polymorphic lookup: "attachments for this entity" is the hot query.
        builder.HasIndex(a => new { a.EntityType, a.EntityId });

        builder.HasOne(a => a.UploadedBy).WithMany().HasForeignKey(a => a.UploadedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(a => a.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
