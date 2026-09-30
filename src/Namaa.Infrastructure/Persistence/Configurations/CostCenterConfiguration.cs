using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.ToTable("CostCenters", "dbo");
        builder.HasKey(e => e.Id).HasName("PK_CostCenters");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.CostCenterCode).HasColumnName("CostCenterCode").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.ParentCostCenterId).HasColumnName("ParentCostCenterId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.BranchId).HasColumnName("BranchId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnType("bit").HasDefaultValueSql("((1))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.CostCenterCode).HasName("UQ_CostCenters_Code");
        builder.HasIndex(e => e.BranchId, "IX_CostCenters_BranchId");
        builder.HasOne(e => e.Branch).WithMany(e => e.CostCenters).HasForeignKey(e => e.BranchId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_CostCenters_Branches");
        builder.HasOne(e => e.ParentCostCenter).WithMany(e => e.ChildCostCenters).HasForeignKey(e => e.ParentCostCenterId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_CostCenters_Parent");
    }
}
