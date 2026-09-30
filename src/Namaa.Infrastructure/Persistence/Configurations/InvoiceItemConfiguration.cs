using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems", "dbo", table =>
        {
            table.HasCheckConstraint("CK_InvoiceItems_Amounts", "(([Quantity]>(0) AND [UnitPrice]>=(0) AND [DiscountAmount]>=(0) AND [DiscountAmount]<=[Quantity]*[UnitPrice] AND [TaxAmount]>=(0) AND [LineTotal]=(([Quantity]*[UnitPrice]-[DiscountAmount])+[TaxAmount])))");
            table.HasCheckConstraint("CK_InvoiceItems_Description", "((len(ltrim(rtrim([Description])))>(0)))");
            table.HasCheckConstraint("CK_InvoiceItems_LineNumber", "(([LineNumber]>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_InvoiceItems");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.InvoiceId).HasColumnName("InvoiceId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.LineNumber).HasColumnName("LineNumber").HasColumnType("smallint").IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("Quantity").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnName("UnitPrice").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.LineTotal).HasColumnName("LineTotal").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.ServiceId).HasColumnName("ServiceId").HasColumnType("bigint").IsRequired(false);
        builder.HasAlternateKey(e => new { e.InvoiceId, e.LineNumber }).HasName("UQ_InvoiceItems_Line");
        builder.HasIndex(e => new { e.InvoiceId, e.LineNumber }, "IX_InvoiceItems_Invoice").IncludeProperties(e => new { e.Description, e.Quantity, e.UnitPrice, e.LineTotal });
        builder.HasIndex(e => e.ServiceId, "IX_InvoiceItems_ServiceId");
        builder.HasOne(e => e.Invoice).WithMany(e => e.Items).HasForeignKey(e => e.InvoiceId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_InvoiceItems_Invoices");
        builder.HasOne(e => e.Service).WithMany(e => e.InvoiceItems).HasForeignKey(e => e.ServiceId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_InvoiceItems_Services");
    }
}
