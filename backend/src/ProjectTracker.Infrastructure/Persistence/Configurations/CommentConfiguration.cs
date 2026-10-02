using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Body).HasMaxLength(10000).IsRequired();
        builder.Property(c => c.RowVersion).IsRowVersion();
        builder.HasIndex(c => c.TaskId);
        builder.HasIndex(c => c.OrganizationId);
        builder.HasIndex(c => c.ParentCommentId);

        // Cascade from Task to its comments (direct aggregate ownership). ParentComment is a
        // self-reference within the same table, so it must be Restrict to avoid a cascade cycle.
        builder.HasOne(c => c.Task).WithMany(t => t.Comments).HasForeignKey(c => c.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(c => c.ParentComment).WithMany(c => c.Replies).HasForeignKey(c => c.ParentCommentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Author).WithMany().HasForeignKey(c => c.AuthorUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(c => c.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class CommentMentionConfiguration : IEntityTypeConfiguration<CommentMention>
{
    public void Configure(EntityTypeBuilder<CommentMention> builder)
    {
        builder.ToTable("CommentMentions");
        builder.HasKey(m => new { m.CommentId, m.UserId });

        builder.HasOne(m => m.Comment).WithMany(c => c.Mentions).HasForeignKey(m => m.CommentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
