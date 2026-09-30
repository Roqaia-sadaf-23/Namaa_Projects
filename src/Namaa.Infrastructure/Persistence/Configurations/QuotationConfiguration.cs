using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("Quotations", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Quotations_Amounts", "(([SubTotal]>=(0) AND [DiscountAmount]>=(0) AND [DiscountAmount]<=[SubTotal] AND [TaxAmount]>=(0) AND [TotalAmount]=(([SubTotal]-[DiscountAmount])+[TaxAmount])))");
            table.HasCheckConstraint("CK_Quotations_Number_NotEmpty", "((len(ltrim(rtrim([QuotationNumber])))>(0)))");
            table.HasCheckConstraint("CK_Quotations_Status", "(([Status]='Cancelled' OR [Status]='ConvertedToProject' OR [Status]='Expired' OR [Status]='Rejected' OR [Status]='Approved' OR [Status]='Sent' OR [Status]='Draft'))");
            table.HasCheckConstraint("CK_Quotations_StatusDates", "((([Status]<>'Sent' OR [SentAt] IS NOT NULL) AND ([Status]<>'Approved' OR [ApprovedAt] IS NOT NULL) AND ([Status]<>'Rejected' OR [RejectedAt] IS NOT NULL)))");
            table.HasCheckConstraint("CK_Quotations_Title_NotEmpty", "((len(ltrim(rtrim([Title])))>(0)))");
            table.HasCheckConstraint("CK_Quotations_Validity", "(([ValidUntil] IS NULL OR [ValidUntil]>=[IssueDate]))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Quotations");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.CustomerId).HasColumnName("CustomerId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.QuotationNumber).HasColumnName("QuotationNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Title).HasColumnName("Title").HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasConversion<string>().HasDefaultValueSql("('Draft')").IsRequired();
        builder.Property(e => e.IssueDate).HasColumnName("IssueDate").HasColumnType("date").HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))").IsRequired();
        builder.Property(e => e.ValidUntil).HasColumnName("ValidUntil").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.SubTotal).HasColumnName("SubTotal").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.TotalAmount).HasColumnName("TotalAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(1500)").HasMaxLength(1500).IsRequired(false);
        builder.Property(e => e.Terms).HasColumnName("Terms").HasColumnType("nvarchar(2000)").HasMaxLength(2000).IsRequired(false);
        builder.Property(e => e.SentAt).HasColumnName("SentAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.ApprovedAt).HasColumnName("ApprovedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RejectedAt).HasColumnName("RejectedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.ApprovedByUserId).HasColumnName("ApprovedByUserId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.QuotationNumber).HasName("UQ_Quotations_Number");
        builder.HasIndex(e => new { e.CustomerId, e.IssueDate }, "IX_Quotations_Customer_Date").IsDescending(false, true).IncludeProperties(e => new { e.QuotationNumber, e.Title, e.Status, e.TotalAmount });
        builder.HasIndex(e => new { e.Status, e.IssueDate }, "IX_Quotations_Status_Date").IsDescending(false, true).IncludeProperties(e => new { e.CustomerId, e.QuotationNumber, e.Title, e.TotalAmount });
        builder.HasOne(e => e.ApprovedByUser).WithMany(e => e.ApprovedQuotations).HasForeignKey(e => e.ApprovedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Quotations_ApprovedBy");
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedQuotations).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Quotations_CreatedBy");
        builder.HasOne(e => e.Customer).WithMany(e => e.Quotations).HasForeignKey(e => e.CustomerId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Quotations_Customers");
    }
}
