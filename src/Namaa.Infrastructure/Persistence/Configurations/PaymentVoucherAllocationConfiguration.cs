using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class PaymentVoucherAllocationConfiguration : IEntityTypeConfiguration<PaymentVoucherAllocation>
{
    public void Configure(EntityTypeBuilder<PaymentVoucherAllocation> builder)
    {
        builder.ToTable("PaymentVoucherAllocations", "dbo", table => table.HasCheckConstraint("CK_PaymentVoucherAllocations_Amount", "(([AllocatedAmount]>(0)))"));
        builder.HasKey(e => e.Id).HasName("PK_PaymentVoucherAllocations");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.PaymentVoucherId).HasColumnName("PaymentVoucherId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.PurchaseInvoiceId).HasColumnName("PurchaseInvoiceId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.AllocatedAmount).HasColumnName("AllocatedAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.HasIndex(e => new { e.PaymentVoucherId, e.PurchaseInvoiceId }, "UX_PaymentVoucherAllocations_Voucher_Invoice").IsUnique();
        builder.HasOne(e => e.PurchaseInvoice).WithMany(e => e.PaymentVoucherAllocations).HasForeignKey(e => e.PurchaseInvoiceId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PaymentVoucherAllocations_Invoice");
        builder.HasOne(e => e.PaymentVoucher).WithMany(e => e.Allocations).HasForeignKey(e => e.PaymentVoucherId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_PaymentVoucherAllocations_Voucher");
    }
}
