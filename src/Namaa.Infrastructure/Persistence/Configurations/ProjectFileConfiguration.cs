using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectFileConfiguration : IEntityTypeConfiguration<ProjectFile>
{
    public void Configure(EntityTypeBuilder<ProjectFile> builder)
    {
        builder.ToTable("ProjectFiles", "dbo", table =>
        {
            table.HasCheckConstraint("CK_ProjectFiles_FileName", "((len(ltrim(rtrim([FileName])))>(0)))");
            table.HasCheckConstraint("CK_ProjectFiles_Size", "(([FileSizeBytes]>(0)))");
            table.HasCheckConstraint("CK_ProjectFiles_StorageKey", "((len(ltrim(rtrim([StorageKey])))>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_ProjectFiles");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.UploadedByUserId).HasColumnName("UploadedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.FileName).HasColumnName("FileName").HasColumnType("nvarchar(260)").HasMaxLength(260).IsRequired();
        builder.Property(e => e.StorageKey).HasColumnName("StorageKey").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired();
        builder.Property(e => e.ContentType).HasColumnName("ContentType").HasColumnType("nvarchar(100)").HasMaxLength(100).IsRequired(false);
        builder.Property(e => e.FileSizeBytes).HasColumnName("FileSizeBytes").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(500)").HasMaxLength(500).IsRequired(false);
        builder.Property(e => e.UploadedAt).HasColumnName("UploadedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.HasAlternateKey(e => e.StorageKey).HasName("UQ_ProjectFiles_StorageKey");
        builder.HasIndex(e => new { e.ProjectId, e.UploadedAt }, "IX_ProjectFiles_Project_UploadedAt").IsDescending(false, true).IncludeProperties(e => new { e.FileName, e.ContentType, e.FileSizeBytes, e.UploadedByUserId });
        builder.HasOne(e => e.Project).WithMany(e => e.Files).HasForeignKey(e => e.ProjectId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectFiles_Projects");
        builder.HasOne(e => e.UploadedByUser).WithMany(e => e.ProjectFiles).HasForeignKey(e => e.UploadedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectFiles_UploadedBy");
    }
}
