using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects", "dbo", table =>
        {
            table.HasCheckConstraint("CK_Projects_Budget", "(([BudgetAmount]>=(0)))");
            table.HasCheckConstraint("CK_Projects_Code_NotEmpty", "((len(ltrim(rtrim([ProjectCode])))>(0)))");
            table.HasCheckConstraint("CK_Projects_Completion", "(([ActualCompletionDate] IS NULL OR [StartDate] IS NULL OR [ActualCompletionDate]>=[StartDate]))");
            table.HasCheckConstraint("CK_Projects_Dates", "(([EndDate] IS NULL OR [StartDate] IS NULL OR [EndDate]>=[StartDate]))");
            table.HasCheckConstraint("CK_Projects_Name_NotEmpty", "((len(ltrim(rtrim([Name])))>(0)))");
            table.HasCheckConstraint("CK_Projects_Progress", "(([ProgressPercentage]>=(0) AND [ProgressPercentage]<=(100)))");
            table.HasCheckConstraint("CK_Projects_Status", "(([Status]='Cancelled' OR [Status]='Completed' OR [Status]='OnHold' OR [Status]='InProgress' OR [Status]='Planned'))");
        });
        builder.HasKey(e => e.Id).HasName("PK_Projects");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.CustomerId).HasColumnName("CustomerId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.SourceQuotationId).HasColumnName("SourceQuotationId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.ProjectManagerId).HasColumnName("ProjectManagerId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.ProjectCode).HasColumnName("ProjectCode").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(2000)").HasMaxLength(2000).IsRequired(false);
        builder.Property(e => e.Location).HasColumnName("Location").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.StartDate).HasColumnName("StartDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.EndDate).HasColumnName("EndDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.ActualCompletionDate).HasColumnName("ActualCompletionDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasConversion<string>().HasDefaultValueSql("('Planned')").IsRequired();
        builder.Property(e => e.ProgressPercentage).HasColumnName("ProgressPercentage").HasColumnType("decimal(5,2)").HasPrecision(5, 2).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.BudgetAmount).HasColumnName("BudgetAmount").HasColumnType("decimal(19,4)").HasPrecision(19, 4).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.ProjectCode).HasName("UQ_Projects_Code");
        builder.HasIndex(e => new { e.CustomerId, e.Status }, "IX_Projects_Customer_Status").IncludeProperties(e => new { e.ProjectCode, e.Name, e.StartDate, e.EndDate, e.ProgressPercentage, e.BudgetAmount });
        builder.HasIndex(e => new { e.ProjectManagerId, e.Status }, "IX_Projects_Manager_Status").IncludeProperties(e => new { e.ProjectCode, e.Name, e.EndDate, e.ProgressPercentage }).HasFilter("[ProjectManagerId] IS NOT NULL");
        builder.HasIndex(e => new { e.Status, e.EndDate }, "IX_Projects_Status_EndDate").IncludeProperties(e => new { e.ProjectCode, e.Name, e.ProjectManagerId, e.ProgressPercentage });
        builder.HasIndex(e => e.SourceQuotationId, "UX_Projects_SourceQuotation").IsUnique().HasFilter("[SourceQuotationId] IS NOT NULL");
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedProjects).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Projects_CreatedBy");
        builder.HasOne(e => e.Customer).WithMany(e => e.Projects).HasForeignKey(e => e.CustomerId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Projects_Customers");
        builder.HasOne(e => e.ProjectManager).WithMany(e => e.ManagedProjects).HasForeignKey(e => e.ProjectManagerId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Projects_Manager");
        builder.HasOne(e => e.SourceQuotation).WithOne(e => e.Project).HasForeignKey<Project>(e => e.SourceQuotationId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_Projects_Quotations");
    }
}
