using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.ToTable("PurchaseInvoices", "dbo", table =>
        {
            table.HasCheckConstraint("CK_PurchaseInvoices_Amounts", "(([SubTotal]>=(0) AND [DiscountAmount]>=(0) AND [TaxAmount]>=(0) AND [TotalAmount]>=(0)))");
            table.HasCheckConstraint("CK_PurchaseInvoices_Dates", "(([DueDate] IS NULL OR [DueDate]>=[IssueDate]))");
            table.HasCheckConstraint("CK_PurchaseInvoices_Status", "(([Status]='Cancelled' OR [Status]='Posted' OR [Status]='Draft'))");
        });
        builder.HasKey(e => e.Id).HasName("PK_PurchaseInvoices");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.SupplierId).HasColumnName("SupplierId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.PurchaseInvoiceNumber).HasColumnName("PurchaseInvoiceNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.SupplierInvoiceNumber).HasColumnName("SupplierInvoiceNumber").HasColumnType("varchar(100)").HasMaxLength(100).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasDefaultValueSql("('Draft')").IsRequired();
        builder.Property(e => e.IssueDate).HasColumnName("IssueDate").HasColumnType("date").IsRequired();
        builder.Property(e => e.DueDate).HasColumnName("DueDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.SubTotal).HasColumnName("SubTotal").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TotalAmount).HasColumnName("TotalAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(1500)").HasMaxLength(1500).IsRequired(false);
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.PostedAt).HasColumnName("PostedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.PostedByUserId).HasColumnName("PostedByUserId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.PurchaseInvoiceNumber).HasName("UQ_PurchaseInvoices_Number");
        builder.HasIndex(e => e.ProjectId, "IX_PurchaseInvoices_ProjectId");
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedPurchaseInvoices).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PurchaseInvoices_CreatedByUser");
        builder.HasOne(e => e.PostedByUser).WithMany(e => e.PostedPurchaseInvoices).HasForeignKey(e => e.PostedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PurchaseInvoices_PostedByUser");
        builder.HasOne(e => e.Project).WithMany(e => e.PurchaseInvoices).HasForeignKey(e => e.ProjectId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PurchaseInvoices_Projects");
        builder.HasOne(e => e.Supplier).WithMany(e => e.PurchaseInvoices).HasForeignKey(e => e.SupplierId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PurchaseInvoices_Suppliers");
    }
}
