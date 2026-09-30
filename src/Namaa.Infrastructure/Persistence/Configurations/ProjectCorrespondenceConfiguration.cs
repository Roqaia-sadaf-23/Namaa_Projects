using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectCorrespondenceConfiguration : IEntityTypeConfiguration<ProjectCorrespondence>
{
    public void Configure(EntityTypeBuilder<ProjectCorrespondence> builder)
    {
        builder.ToTable("ProjectCorrespondences", "dbo", table => table.HasCheckConstraint("CK_ProjectCorrespondences_Type", "(([CorrespondenceType]='Outgoing' OR [CorrespondenceType]='Incoming'))"));
        builder.HasKey(e => e.Id).HasName("PK_ProjectCorrespondences");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CorrespondenceType).HasColumnName("CorrespondenceType").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(e => e.Subject).HasColumnName("Subject").HasColumnType("nvarchar(300)").HasMaxLength(300).IsRequired();
        builder.Property(e => e.ReferenceNumber).HasColumnName("ReferenceNumber").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.CorrespondenceDate).HasColumnName("CorrespondenceDate").HasColumnType("date").IsRequired();
        builder.Property(e => e.Notes).HasColumnName("Notes").HasColumnType("nvarchar(2000)").HasMaxLength(2000).IsRequired(false);
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.HasOne(e => e.Project).WithMany(e => e.Correspondences).HasForeignKey(e => e.ProjectId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectCorrespondences_Project");
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedProjectCorrespondences).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectCorrespondences_User");
    }
}
