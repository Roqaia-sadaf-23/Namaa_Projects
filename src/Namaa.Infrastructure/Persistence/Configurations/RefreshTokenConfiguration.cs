using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "dbo", table =>
        {
            table.HasCheckConstraint("CK_RefreshTokens_Expiry", "(([ExpiresAt]>[CreatedAt]))");
            table.HasCheckConstraint("CK_RefreshTokens_Revocation", "(([RevokedAt] IS NULL OR [RevokedAt]>=[CreatedAt]))");
        });
        builder.HasKey(e => e.Id).HasName("PK_RefreshTokens");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.TokenHash).HasColumnName("TokenHash").HasColumnType("char(64)").HasMaxLength(64).IsUnicode(false).IsFixedLength().IsRequired();
        builder.Property(e => e.ExpiresAt).HasColumnName("ExpiresAt").HasColumnType("datetime2(0)").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.RevokedAt).HasColumnName("RevokedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.ReplacedById).HasColumnName("ReplacedById").HasColumnType("bigint").IsRequired(false);
        builder.HasAlternateKey(e => e.TokenHash).HasName("UQ_RefreshTokens_TokenHash");
        builder.HasIndex(e => new { e.UserId, e.ExpiresAt }, "IX_RefreshTokens_User_ExpiresAt").IsDescending(false, true).IncludeProperties(e => e.RevokedAt);
        builder.HasOne(e => e.ReplacedBy).WithMany(e => e.ReplacementFor).HasForeignKey(e => e.ReplacedById).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_RefreshTokens_ReplacedBy");
        builder.HasOne(e => e.User).WithMany(e => e.RefreshTokens).HasForeignKey(e => e.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_RefreshTokens_Users");
    }
}
