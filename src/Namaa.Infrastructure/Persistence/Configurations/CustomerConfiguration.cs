using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Customers_Code_NotEmpty", "((len(ltrim(rtrim([CustomerCode])))>(0)))");
            table.HasCheckConstraint("CK_Customers_Name_NotEmpty", "((len(ltrim(rtrim([Name])))>(0)))");
            table.HasCheckConstraint("CK_Customers_Type", "(([CustomerType]='Company' OR [CustomerType]='Individual'))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Customers");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.CustomerCode).HasColumnName("CustomerCode").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.CustomerType).HasColumnName("CustomerType").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasDefaultValueSql("('Company')").IsRequired();
        builder.Property(e => e.Email).HasColumnName("Email").HasColumnType("nvarchar(254)").HasMaxLength(254).IsRequired(false);
        builder.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").HasColumnType("nvarchar(30)").HasMaxLength(30).IsRequired(false);
        builder.Property(e => e.Address).HasColumnName("Address").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.City).HasColumnName("City").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.TaxNumber).HasColumnName("TaxNumber").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.CommercialRegister).HasColumnName("CommercialRegister").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.Source).HasColumnName("Source").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValueSql("((1))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.CustomerCode).HasName("UQ_Customers_Code");
        builder.HasIndex(e => new { e.Name, e.IsActive }, "IX_Customers_Name_Active").IncludeProperties(e => new { e.CustomerCode, e.Email, e.PhoneNumber });
    }
}
