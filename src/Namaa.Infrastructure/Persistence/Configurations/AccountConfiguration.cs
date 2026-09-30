using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Accounts_AccountType", "(([AccountType]='Expense' OR [AccountType]='Revenue' OR [AccountType]='Equity' OR [AccountType]='Liability' OR [AccountType]='Asset'))");
            table.HasCheckConstraint("CK_Accounts_Name_NotEmpty", "((len(ltrim(rtrim([AccountName])))>(0)))");
        });

        builder.HasKey(e => e.Id).HasName("PK_Accounts");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.AccountCode).HasColumnName("AccountCode").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.AccountName).HasColumnName("AccountName").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.ParentAccountId).HasColumnName("ParentAccountId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.AccountType).HasColumnName("AccountType").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(e => e.IsPostingAllowed).HasColumnName("IsPostingAllowed").HasColumnType("bit").HasDefaultValueSql("((1))").IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValueSql("((1))").IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();

        builder.HasAlternateKey(e => e.AccountCode).HasName("UQ_Accounts_AccountCode");
        builder.HasIndex(e => e.ParentAccountId, "IX_Accounts_ParentAccountId");
        builder.HasOne(e => e.ParentAccount).WithMany(e => e.ChildAccounts).HasForeignKey(e => e.ParentAccountId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Accounts_ParentAccount");
    }
}
