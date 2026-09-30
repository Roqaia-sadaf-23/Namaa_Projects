using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines", "dbo", table => table.HasCheckConstraint("CK_JournalEntryLines_DebitCredit", "(([Debit]>(0) AND [Credit]=(0) OR [Credit]>(0) AND [Debit]=(0)))"));
        builder.HasKey(e => e.Id).HasName("PK_JournalEntryLines");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.JournalEntryId).HasColumnName("JournalEntryId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.LineNumber).HasColumnName("LineNumber").HasColumnType("smallint").IsRequired();
        builder.Property(e => e.AccountId).HasColumnName("AccountId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.BranchId).HasColumnName("BranchId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.CostCenterId).HasColumnName("CostCenterId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.Debit).HasColumnName("Debit").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.Credit).HasColumnName("Credit").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.HasIndex(e => e.AccountId, "IX_JournalEntryLines_AccountId");
        builder.HasIndex(e => new { e.JournalEntryId, e.LineNumber }, "UX_JournalEntryLines_Entry_Line").IsUnique();
        builder.HasOne(e => e.Account).WithMany(e => e.JournalEntryLines).HasForeignKey(e => e.AccountId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntryLines_Accounts");
        builder.HasOne(e => e.Branch).WithMany(e => e.JournalEntryLines).HasForeignKey(e => e.BranchId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntryLines_Branches");
        builder.HasOne(e => e.CostCenter).WithMany(e => e.JournalEntryLines).HasForeignKey(e => e.CostCenterId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntryLines_CostCenters");
        builder.HasOne(e => e.JournalEntry).WithMany(e => e.Lines).HasForeignKey(e => e.JournalEntryId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntryLines_JournalEntries");
        builder.HasOne(e => e.Project).WithMany(e => e.JournalEntryLines).HasForeignKey(e => e.ProjectId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntryLines_Projects");
    }
}
