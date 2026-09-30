using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "dbo", table =>
        {
            table.HasCheckConstraint("CK_AuditLogs_Action", "((len(ltrim(rtrim([Action])))>(0)))");
            table.HasCheckConstraint("CK_AuditLogs_EntityType", "((len(ltrim(rtrim([EntityType])))>(0)))");
            table.HasCheckConstraint("CK_AuditLogs_Json", "((([OldValues] IS NULL OR isjson([OldValues])=(1)) AND ([NewValues] IS NULL OR isjson([NewValues])=(1))))");
        });

        builder.HasKey(e => e.Id).HasName("PK_AuditLogs");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.Action).HasColumnName("Action").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(e => e.EntityType).HasColumnName("EntityType").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("EntityId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.OldValues).HasColumnName("OldValues").HasColumnType("nvarchar(max)").IsRequired(false);
        builder.Property(e => e.NewValues).HasColumnName("NewValues").HasColumnType("nvarchar(max)").IsRequired(false);
        builder.Property(e => e.IpAddress).HasColumnName("IpAddress").HasColumnType("varchar(45)").HasMaxLength(45).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();

        builder.HasIndex(e => new { e.EntityType, e.EntityId, e.CreatedAt }, "IX_AuditLogs_Entity").IsDescending(false, false, true).IncludeProperties(e => new { e.UserId, e.Action });
        builder.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_AuditLogs_User_Date").IsDescending(false, true).IncludeProperties(e => new { e.Action, e.EntityType, e.EntityId }).HasFilter("[UserId] IS NOT NULL");
        builder.HasOne(e => e.User).WithMany(e => e.AuditLogs).HasForeignKey(e => e.UserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_AuditLogs_Users");
    }
}
