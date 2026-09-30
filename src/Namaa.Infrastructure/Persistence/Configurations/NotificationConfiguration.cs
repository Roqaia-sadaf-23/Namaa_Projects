using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Notifications_Entity", "(([EntityType] IS NULL AND [EntityId] IS NULL OR [EntityType] IS NOT NULL AND [EntityId] IS NOT NULL))");
            table.HasCheckConstraint("CK_Notifications_Message", "((len(ltrim(rtrim([Message])))>(0)))");
            table.HasCheckConstraint("CK_Notifications_Read", "(([IsRead]=(0) AND [ReadAt] IS NULL OR [IsRead]=(1) AND [ReadAt] IS NOT NULL))");
            table.HasCheckConstraint("CK_Notifications_Title", "((len(ltrim(rtrim([Title])))>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Notifications");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.Title).HasColumnName("Title").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Message).HasColumnName("Message").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired();
        builder.Property(e => e.EntityType).HasColumnName("EntityType").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.EntityId).HasColumnName("EntityId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.IsRead).HasColumnName("IsRead").HasColumnType("bit").HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.ReadAt).HasColumnName("ReadAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt }, "IX_Notifications_User_Unread").IsDescending(false, false, true).IncludeProperties(e => new { e.Title, e.EntityType, e.EntityId, e.ReadAt });
        builder.HasOne(e => e.User).WithMany(e => e.Notifications).HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Notifications_Users");
    }
}
