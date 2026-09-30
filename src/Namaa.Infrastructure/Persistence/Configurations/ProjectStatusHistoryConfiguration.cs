using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectStatusHistoryConfiguration : IEntityTypeConfiguration<ProjectStatusHistory>
{
    public void Configure(EntityTypeBuilder<ProjectStatusHistory> builder)
    {
        builder.ToTable("ProjectStatusHistory", "dbo");
        builder.HasKey(e => e.Id).HasName("PK_ProjectStatusHistory");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.OldStatus).HasColumnName("OldStatus").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.NewStatus).HasColumnName("NewStatus").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(e => e.Reason).HasColumnName("Reason").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.ChangedByUserId).HasColumnName("ChangedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.ChangedAt).HasColumnName("ChangedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.HasIndex(e => e.ProjectId, "IX_ProjectStatusHistory_ProjectId");
        builder.HasOne(e => e.Project).WithMany(e => e.StatusHistoryEntries).HasForeignKey(e => e.ProjectId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectStatusHistory_Projects");
        builder.HasOne(e => e.ChangedByUser).WithMany(e => e.ProjectStatusChanges).HasForeignKey(e => e.ChangedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectStatusHistory_Users");
    }
}
