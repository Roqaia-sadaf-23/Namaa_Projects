using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Configurations;

public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("ProjectTasks", "dbo", table =>
        {
            table.HasCheckConstraint("CK_ProjectTasks_Completion", "(([Status]='Completed' AND [CompletedAt] IS NOT NULL AND [ProgressPercentage]=(100) OR [Status]<>'Completed' AND [CompletedAt] IS NULL))");
            table.HasCheckConstraint("CK_ProjectTasks_Dates", "(([DueDate] IS NULL OR [StartDate] IS NULL OR [DueDate]>=[StartDate]))");
            table.HasCheckConstraint("CK_ProjectTasks_Priority", "(([Priority]='Urgent' OR [Priority]='High' OR [Priority]='Medium' OR [Priority]='Low'))");
            table.HasCheckConstraint("CK_ProjectTasks_Progress", "(([ProgressPercentage]>=(0) AND [ProgressPercentage]<=(100)))");
            table.HasCheckConstraint("CK_ProjectTasks_Status", "(([Status]='Cancelled' OR [Status]='Completed' OR [Status]='InProgress' OR [Status]='ToDo'))");
            table.HasCheckConstraint("CK_ProjectTasks_TaskType", "(([TaskType] IS NULL OR ([TaskType]='Administrative' OR [TaskType]='Technical')))");
            table.HasCheckConstraint("CK_ProjectTasks_Title_NotEmpty", "((len(ltrim(rtrim([Title])))>(0)))");
        });
        builder.HasKey(e => e.Id).HasName("PK_ProjectTasks");
        builder.Property(e => e.Id).HasColumnName("Id").HasColumnType("bigint").UseIdentityColumn(1, 1);
        builder.Property(e => e.ProjectId).HasColumnName("ProjectId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.ParentTaskId).HasColumnName("ParentTaskId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.AssignedToUserId).HasColumnName("AssignedToUserId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.Title).HasColumnName("Title").HasColumnType("nvarchar(250)").HasMaxLength(250).IsRequired();
        builder.Property(e => e.Description).HasColumnName("Description").HasColumnType("nvarchar(2000)").HasMaxLength(2000).IsRequired(false);
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).HasConversion<string>().HasDefaultValueSql("('ToDo')").IsRequired();
        builder.Property(e => e.Priority).HasColumnName("Priority").HasColumnType("varchar(10)").HasMaxLength(10).IsUnicode(false).HasConversion<string>().HasDefaultValueSql("('Medium')").IsRequired();
        builder.Property(e => e.StartDate).HasColumnName("StartDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.DueDate).HasColumnName("DueDate").HasColumnType("date").IsRequired(false);
        builder.Property(e => e.CompletedAt).HasColumnName("CompletedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.ProgressPercentage).HasColumnName("ProgressPercentage").HasColumnType("decimal(5,2)").HasPrecision(5, 2).HasDefaultValueSql("((0))").IsRequired();
        builder.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId").HasColumnType("bigint").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("(sysutcdatetime())").IsRequired();
        builder.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasColumnType("datetime2(0)").IsRequired(false);
        builder.Property(e => e.RowVersion).HasColumnName("RowVersion").HasColumnType("timestamp").IsRowVersion().IsConcurrencyToken().IsRequired();
        builder.Property(e => e.StageId).HasColumnName("StageId").HasColumnType("bigint").IsRequired(false);
        builder.Property(e => e.TaskType).HasColumnName("TaskType").HasColumnType("varchar(20)").HasMaxLength(20).IsUnicode(false).IsRequired(false);
        builder.HasIndex(e => new { e.AssignedToUserId, e.Status, e.DueDate }, "IX_ProjectTasks_Assignee_Status").IncludeProperties(e => new { e.ProjectId, e.Title, e.Priority, e.ProgressPercentage }).HasFilter("[AssignedToUserId] IS NOT NULL");
        builder.HasIndex(e => new { e.ProjectId, e.Status, e.DueDate }, "IX_ProjectTasks_Project_Status").IncludeProperties(e => new { e.Title, e.AssignedToUserId, e.Priority, e.ProgressPercentage });
        builder.HasOne(e => e.AssignedToUser).WithMany(e => e.AssignedTasks).HasForeignKey(e => e.AssignedToUserId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectTasks_AssignedTo");
        builder.HasOne(e => e.CreatedByUser).WithMany(e => e.CreatedTasks).HasForeignKey(e => e.CreatedByUserId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectTasks_CreatedBy");
        builder.HasOne(e => e.ParentTask).WithMany(e => e.ChildTasks).HasForeignKey(e => e.ParentTaskId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectTasks_Parent");
        builder.HasOne(e => e.Project).WithMany(e => e.Tasks).HasForeignKey(e => e.ProjectId).IsRequired().OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectTasks_Projects");
        builder.HasOne(e => e.Stage).WithMany(e => e.Tasks).HasForeignKey(e => e.StageId).IsRequired(false).OnDelete(DeleteBehavior.NoAction).HasConstraintName("FK_ProjectTasks_ProjectStages");
    }
}
