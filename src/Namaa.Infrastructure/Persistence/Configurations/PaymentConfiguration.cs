using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Payments_Amount", "(([Amount]>(0)))");
            table.HasCheckConstraint("CK_Payments_Method", "(([PaymentMethod]='Other' OR [PaymentMethod]='BankTransfer' OR [PaymentMethod]='Card' OR [PaymentMethod]='Cash'))");
            table.HasCheckConstraint("CK_Payments_Number_NotEmpty", "((len(ltrim(rtrim([PaymentNumber])))>(0)))");
            table.HasCheckConstraint("CK_Payments_Status", "(([Status]='Cancelled' OR [Status]='Posted' OR [Status]='Draft'))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Payments");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.InvoiceId).HasColumnName("InvoiceId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.PaymentNumber).HasColumnName("PaymentNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.PaymentDate).HasColumnName("PaymentDate").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.PaymentMethod).HasColumnName("PaymentMethod").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasConversion<string>().IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.ReferenceNumber).HasColumnName("ReferenceNumber").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.ReceivedByUserId).HasColumnName("ReceivedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.Property(e => e.CustomerId).HasColumnName("CustomerId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasDefaultValueSql("('Draft')").IsRequired();
        builder.Property(e => e.PostedAt).HasColumnName("PostedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.PostedByUserId).HasColumnName("PostedByUserId").HasColumnType("bigint").IsRequired(false);
        builder.HasAlternateKey(e => e.PaymentNumber).HasName("UQ_Payments_Number");
        builder.HasIndex(e => e.PaymentDate, "IX_Payments_Date").IsDescending(true).IncludeProperties(e => new { e.InvoiceId, e.PaymentNumber, e.PaymentMethod, e.Amount });
        builder.HasIndex(e => new { e.InvoiceId, e.PaymentDate }, "IX_Payments_Invoice_Date").IsDescending(false, true).IncludeProperties(e => new { e.PaymentNumber, e.PaymentMethod, e.Amount, e.ReferenceNumber });
        builder.HasOne(e => e.Customer).WithMany(e => e.Payments).HasForeignKey(e => e.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Payments_Customers");
        builder.HasOne(e => e.Invoice).WithMany(e => e.Payments).HasForeignKey(e => e.InvoiceId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Payments_Invoices");
        builder.HasOne(e => e.PostedByUser).WithMany(e => e.PostedPayments).HasForeignKey(e => e.PostedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Payments_PostedByUser");
        builder.HasOne(e => e.ReceivedByUser).WithMany(e => e.PaymentsReceived).HasForeignKey(e => e.ReceivedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Payments_ReceivedBy");
    }
}
