using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers", "dbo", table => table.HasCheckConstraint("CK_ProjectMembers_Dates", "(([LeftAt] IS NULL OR [LeftAt]>=[JoinedAt]))"));
        builder.HasKey(e => e.Id).HasName("PK_ProjectMembers");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.ProjectRole).HasColumnName("ProjectRole").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.JoinedAt).HasColumnName("JoinedAt").HasColumnType("date").HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))").IsRequired();
        builder.Property(e => e.LeftAt).HasColumnName("LeftAt").HasColumnType("date").IsRequired(false);
        builder.HasAlternateKey(e => new { e.ProjectId, e.UserId }).HasName("UQ_ProjectMembers_Project_User");
        builder.HasIndex(e => new { e.UserId, e.LeftAt }, "IX_ProjectMembers_User").IncludeProperties(e => new { e.ProjectId, e.ProjectRole, e.JoinedAt });
        builder.HasIndex(e => new { e.ProjectId, e.UserId }, "UX_ProjectMembers_ActiveMember").IsUnique().HasFilter("[LeftAt] IS NULL");
        builder.HasOne(e => e.Project).WithMany(e => e.Members).HasForeignKey(e => e.ProjectId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectMembers_Projects");
        builder.HasOne(e => e.User).WithMany(e => e.ProjectMemberships).HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectMembers_Users");
    }
}
