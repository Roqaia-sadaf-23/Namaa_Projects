using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Users_Contact", "(([Email] IS NOT NULL OR [PhoneNumber] IS NOT NULL))");
            table.HasCheckConstraint("CK_Users_FullName_NotEmpty", "((len(ltrim(rtrim([FullName])))>(0)))");
            table.HasCheckConstraint("CK_Users_PasswordHash_NotEmpty", "((len(ltrim(rtrim([PasswordHash])))>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Users");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.RoleId).HasColumnName("RoleId").HasColumnType("tinyint").IsRequired();
        builder.Property(e => e.FullName).HasColumnName("FullName").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Email).HasColumnName("Email").HasColumnType("nvarchar(254)").HasMaxLength(254).IsRequired(false);
        builder.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").HasColumnType("nvarchar(30)").HasMaxLength(30).IsRequired(false);
        builder.Property(e => e.PasswordHash).HasColumnName("PasswordHash").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValueSql("((1))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasIndex(e => new { e.RoleId, e.IsActive }, "IX_Users_Role_Active").IncludeProperties(e => new { e.FullName, e.Email, e.PhoneNumber });
        builder.HasIndex(e => e.Email, "UX_Users_Email").IsUnique().HasFilter("[Email] IS NOT NULL");
        builder.HasIndex(e => e.PhoneNumber, "UX_Users_PhoneNumber").IsUnique().HasFilter("[PhoneNumber] IS NOT NULL");
        builder.HasOne(e => e.Role).WithMany(e => e.Users).HasForeignKey(e => e.RoleId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Users_Roles");
    }
}
