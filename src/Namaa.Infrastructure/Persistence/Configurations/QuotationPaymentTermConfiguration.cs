using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class QuotationPaymentTermConfiguration : IEntityTypeConfiguration<QuotationPaymentTerm>
{
    public void Configure(EntityTypeBuilder<QuotationPaymentTerm> builder)
    {
        builder.ToTable("QuotationPaymentTerms", "dbo", table =>
        {
            table.HasCheckConstraint("CK_QuotationPaymentTerms_Amount", "(([Amount] IS NULL OR [Amount]>=(0)))");
            table.HasCheckConstraint("CK_QuotationPaymentTerms_DueAfterDays", "(([DueAfterDays] IS NULL OR [DueAfterDays]>=(0)))");
            table.HasCheckConstraint("CK_QuotationPaymentTerms_InstallmentNumber", "(([InstallmentNumber]>(0)))");
            table.HasCheckConstraint("CK_QuotationPaymentTerms_Percentage", "(([Percentage] IS NULL OR [Percentage]>=(0) AND [Percentage]<=(100)))");
            table.HasCheckConstraint("CK_QuotationPaymentTerms_Value", "(([Percentage] IS NOT NULL OR [Amount] IS NOT NULL))");
        });
        builder.HasKey(e => e.Id).HasName("PK_QuotationPaymentTerms");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.QuotationId).HasColumnName("QuotationId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.InstallmentNumber).HasColumnName("InstallmentNumber").HasColumnType("smallint").IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.Percentage).HasColumnName("Percentage").HasColumnType("decimal(5,2)").HasPrecision(5, 2).IsRequired(false);
        builder.Property(e => e.Amount).HasColumnName("Amount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired(false);
        builder.Property(e => e.DueAfterDays).HasColumnName("DueAfterDays").HasColumnType("int").IsRequired(false);
        builder.Property(e => e.DueDate).HasColumnName("DueDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.HasIndex(e => new { e.QuotationId, e.InstallmentNumber }, "UX_QuotationPaymentTerms_Quotation_Installment").IsUnique();
        builder.HasOne(e => e.Quotation).WithMany(e => e.PaymentTerms).HasForeignKey(e => e.QuotationId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_QuotationPaymentTerms_Quotations");
    }
}
