using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers", "dbo", table => table.HasCheckConstraint("CK_Suppliers_Name_NotEmpty", "((len(ltrim(rtrim([Name])))>(0)))"));
        builder.HasKey(e => e.Id).HasName("PK_Suppliers");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.SupplierCode).HasColumnName("SupplierCode").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.Email).HasColumnName("Email").HasColumnType("nvarchar(254)").HasMaxLength(254).IsRequired(false);
        builder.Property(e => e.Address).HasColumnName("Address").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.City).HasColumnName("City").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.TaxNumber).HasColumnName("TaxNumber").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.CommercialRegister).HasColumnName("CommercialRegister").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValueSql("((1))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.SupplierCode).HasName("UQ_Suppliers_SupplierCode");
    }
}
