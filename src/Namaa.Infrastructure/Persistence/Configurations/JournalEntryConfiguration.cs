using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries", "dbo", table => table.HasCheckConstraint("CK_JournalEntries_Status", "(([Status]='Cancelled' OR [Status]='Posted' OR [Status]='Draft'))"));
        builder.HasKey(e => e.Id).HasName("PK_JournalEntries");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.EntryNumber).HasColumnName("EntryNumber").HasColumnType("varchar(30)").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.EntryDate).HasColumnName("EntryDate").HasColumnType("date").IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(1000)").HasMaxLength(1000).IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasDefaultValueSql("('Draft')").IsRequired();
        builder.Property(e => e.SourceType).HasColumnName("SourceType").HasColumnType("varchar(50)").HasMaxLength(50).IsUnicode(false).IsRequired(false);
        builder.Property(e => e.SourceId).HasColumnName("SourceId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.PostedByUserId).HasColumnName("PostedByUserId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.PostedAt).HasColumnName("PostedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.HasAlternateKey(e => e.EntryNumber).HasName("UQ_JournalEntries_EntryNumber");
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedJournalEntries).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntries_CreatedByUser");
        builder.HasOne(e => e.PostedByUser).WithMany(e => e.PostedJournalEntries).HasForeignKey(e => e.PostedByUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_JournalEntries_PostedByUser");
    }
}
