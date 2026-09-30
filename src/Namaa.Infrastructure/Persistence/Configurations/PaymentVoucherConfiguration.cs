using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class PaymentVoucherConfiguration : IEntityTypeConfiguration<PaymentVoucher>
{
    public void Configure(EntityTypeBuilder<PaymentVoucher> builder)
    {
        builder.ToTable("PaymentVouchers", "dbo", table =>
        {
            table.HasCheckConstraint("CK_PaymentVouchers_Amount", "(([Amount]>(0)))");
            table.HasCheckConstraint("CK_PaymentVouchers_Beneficiary", "(([SupplierId] IS NOT NULL OR [BeneficiaryName] IS NOT NULL))");
            table.HasCheckConstraint("CK_PaymentVouchers_Method", "(([PaymentMethod]='Other' OR [PaymentMethod]='Cheque' OR [PaymentMethod]='Card' OR [PaymentMethod]='BankTransfer' OR [PaymentMethod]='Cash'))");
            table.HasCheckConstraint("CK_PaymentVouchers_Status", "(([Status]='Cancelled' OR [Status]='Posted' OR [Status]='Draft'))");
        });
        builder.HasKey(e => e.Id).HasName("PK_PaymentVouchers");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.VoucherNumber).HasColumnName("VoucherNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.SupplierId).HasColumnName("SupplierId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.BeneficiaryName).HasColumnName("BeneficiaryName").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired(false);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.VoucherDate).HasColumnName("VoucherDate").HasColumnType("date").IsRequired();
        builder.Property(e => e.PaymentMethod).HasColumnName("PaymentMethod").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.ReferenceNumber).HasColumnName("ReferenceNumber").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasDefaultValueSql("('Draft')").IsRequired();
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.PaidByUserId).HasColumnName("PaidByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.PostedByUserId).HasColumnName("PostedByUserId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.PostedAt).HasColumnName("PostedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.VoucherNumber).HasName("UQ_PaymentVouchers_Number");
        builder.HasOne(e => e.PaidByUser).WithMany(e => e.PaidPaymentVouchers).HasForeignKey(e => e.PaidByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PaymentVouchers_PaidByUser");
        builder.HasOne(e => e.PostedByUser).WithMany(e => e.PostedPaymentVouchers).HasForeignKey(e => e.PostedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PaymentVouchers_PostedByUser");
        builder.HasOne(e => e.Project).WithMany(e => e.PaymentVouchers).HasForeignKey(e => e.ProjectId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PaymentVouchers_Projects");
        builder.HasOne(e => e.Supplier).WithMany(e => e.PaymentVouchers).HasForeignKey(e => e.SupplierId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PaymentVouchers_Suppliers");
    }
}
