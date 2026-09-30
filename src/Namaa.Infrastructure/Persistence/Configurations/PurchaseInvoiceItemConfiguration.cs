using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class PurchaseInvoiceItemConfiguration : IEntityTypeConfiguration<PurchaseInvoiceItem>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoiceItem> builder)
    {
        builder.ToTable("PurchaseInvoiceItems", "dbo", table =>
        {
            table.HasCheckConstraint("CK_PurchaseInvoiceItems_Amounts", "(([DiscountAmount]>=(0) AND [TaxAmount]>=(0) AND [LineTotal]>=(0)))");
            table.HasCheckConstraint("CK_PurchaseInvoiceItems_Quantity", "(([Quantity]>(0)))");
            table.HasCheckConstraint("CK_PurchaseInvoiceItems_UnitPrice", "(([UnitPrice]>=(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_PurchaseInvoiceItems");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.PurchaseInvoiceId).HasColumnName("PurchaseInvoiceId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.LineNumber).HasColumnName("LineNumber").HasColumnType("smallint").IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("Quantity").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnName("UnitPrice").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.LineTotal).HasColumnName("LineTotal").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.HasIndex(e => new { e.PurchaseInvoiceId, e.LineNumber }, "UX_PurchaseInvoiceItems_Invoice_Line").IsUnique();
        builder.HasOne(e => e.PurchaseInvoice).WithMany(e => e.Items).HasForeignKey(e => e.PurchaseInvoiceId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PurchaseInvoiceItems_PurchaseInvoices");
    }
}
