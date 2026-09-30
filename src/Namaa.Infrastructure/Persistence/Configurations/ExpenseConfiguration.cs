using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Expenses_Amount", "(([Amount]>(0)))");
            table.HasCheckConstraint("CK_Expenses_Category_NotEmpty", "((len(ltrim(rtrim([Category])))>(0)))");
            table.HasCheckConstraint("CK_Expenses_Description_NotEmpty", "((len(ltrim(rtrim([Description])))>(0)))");
            table.HasCheckConstraint("CK_Expenses_Number_NotEmpty", "((len(ltrim(rtrim([ExpenseNumber])))>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Expenses");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.ExpenseNumber).HasColumnName("ExpenseNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.ExpenseDate).HasColumnName("ExpenseDate").HasColumnType("date").HasDefaultValueSql("(CONVERT([date],sysutcdatetime()))").IsRequired();
        builder.Property(e => e.Category).HasColumnName("Category").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).IsRequired();
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.ExpenseNumber).HasName("UQ_Expenses_Number");
        builder.HasIndex(e => new { e.ProjectId, e.ExpenseDate }, "IX_Expenses_Project_Date").IsDescending(false, true).IncludeProperties(e => new { e.ExpenseNumber, e.Category, e.Description, e.Amount });
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedExpenses).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Expenses_CreatedBy");
        builder.HasOne(e => e.Project).WithMany(e => e.Expenses).HasForeignKey(e => e.ProjectId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Expenses_Projects");
    }
}
