using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class BoardColumnConfiguration : IEntityTypeConfiguration<BoardColumn>
{
    public void Configure(EntityTypeBuilder<BoardColumn> builder)
    {
        builder.ToTable("BoardColumns");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Color).HasMaxLength(20).IsRequired();
        builder.Property(c => c.StatusMapping).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.RowVersion).IsRowVersion();
        builder.HasIndex(c => new { c.ProjectId, c.Order });

        builder.HasOne(c => c.Project).WithMany().HasForeignKey(c => c.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}
