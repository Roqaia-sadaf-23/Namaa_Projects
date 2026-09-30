using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Invoices_Amounts", "(([SubTotal]>=(0) AND [DiscountAmount]>=(0) AND [DiscountAmount]<=[SubTotal] AND [TaxAmount]>=(0) AND [TotalAmount]=(([SubTotal]-[DiscountAmount])+[TaxAmount])))");
            table.HasCheckConstraint("CK_Invoices_Dates", "(([DueDate] IS NULL OR [DueDate]>=[IssueDate]))");
            table.HasCheckConstraint("CK_Invoices_Number_NotEmpty", "((len(ltrim(rtrim([InvoiceNumber])))>(0)))");
            table.HasCheckConstraint("CK_Invoices_Status", "(([Status]='Cancelled' OR [Status]='Paid' OR [Status]='PartiallyPaid' OR [Status]='Issued' OR [Status]='Draft'))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Invoices");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.InvoiceNumber).HasColumnName("InvoiceNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasConversion<string>().HasDefaultValueSql("('Draft')").IsRequired();
        builder.Property(e => e.IssueDate).HasColumnName("IssueDate").HasColumnType("date").HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))").IsRequired();
        builder.Property(e => e.DueDate).HasColumnName("DueDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.SubTotal).HasColumnName("SubTotal").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TotalAmount).HasColumnName("TotalAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.Property(e => e.CustomerId).HasColumnName("CustomerId").HasColumnType("bigint").IsRequired(false);
        builder.HasAlternateKey(e => e.InvoiceNumber).HasName("UQ_Invoices_Number");
        builder.HasIndex(e => new { e.ProjectId, e.IssueDate }, "IX_Invoices_Project_Date").IsDescending(false, true).IncludeProperties(e => new { e.InvoiceNumber, e.Status, e.DueDate, e.TotalAmount });
        builder.HasIndex(e => new { e.Status, e.DueDate }, "IX_Invoices_Status_DueDate").IncludeProperties(e => new { e.ProjectId, e.InvoiceNumber, e.IssueDate, e.TotalAmount });
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedInvoices).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Invoices_CreatedBy");
        builder.HasOne(e => e.Customer).WithMany(e => e.Invoices).HasForeignKey(e => e.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Invoices_Customers");
        builder.HasOne(e => e.Project).WithMany(e => e.Invoices).HasForeignKey(e => e.ProjectId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Invoices_Projects");
    }
}
