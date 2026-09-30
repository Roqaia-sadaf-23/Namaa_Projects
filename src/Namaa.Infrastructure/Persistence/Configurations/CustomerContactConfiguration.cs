using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("CustomerContacts", "dbo", table =>
        {
            table.HasCheckConstraint("CK_CustomerContacts_Contact", "(([Email] IS NOT NULL OR [PhoneNumber] IS NOT NULL))");
            table.HasCheckConstraint("CK_CustomerContacts_Name_NotEmpty", "((len(ltrim(rtrim([FullName])))>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_CustomerContacts");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.CustomerId).HasColumnName("CustomerId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.FullName).HasColumnName("FullName").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.JobTitle).HasColumnName("JobTitle").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Email).HasColumnName("Email").HasColumnType("nvarchar(254)").HasMaxLength(254).IsRequired(false);
        builder.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").HasColumnType("nvarchar(30)").HasMaxLength(30).IsRequired(false);
        builder.Property(e => e.IsPrimary).HasColumnName("IsPrimary").HasColumnType("bit").HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.HasIndex(e => new { e.CustomerId, e.IsPrimary }, "IX_CustomerContacts_Customer").IsDescending(false, true).IncludeProperties(e => new { e.FullName, e.JobTitle, e.Email, e.PhoneNumber });
        builder.HasIndex(e => e.CustomerId, "UX_CustomerContacts_OnePrimary").IsUnique().HasFilter("[IsPrimary]=(1)");
        builder.HasOne(e => e.Customer).WithMany(e => e.Contacts).HasForeignKey(e => e.CustomerId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_CustomerContacts_Customers");
    }
}
