using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectStageConfiguration : IEntityTypeConfiguration<ProjectStage>
{
    public void Configure(EntityTypeBuilder<ProjectStage> builder)
    {
        builder.ToTable("ProjectStages", "dbo", table => table.HasCheckConstraint("CK_ProjectStages_Progress", "(([ProgressPercentage]>=(0) AND [ProgressPercentage]<=(100)))"));
        builder.HasKey(e => e.Id).HasName("PK_ProjectStages");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasColumnType("nvarchar(200)").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.StageOrder).HasColumnName("StageOrder").HasColumnType("int").IsRequired();
        builder.Property(e => e.StartDate).HasColumnName("StartDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.EndDate).HasColumnName("EndDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(e => e.ProgressPercentage).HasColumnName("ProgressPercentage").HasColumnType("decimal(5,2)").HasPrecision(5, 2).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.HasOne(e => e.Project).WithMany(e => e.Stages).HasForeignKey(e => e.ProjectId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectStages_Projects");
    }
}
