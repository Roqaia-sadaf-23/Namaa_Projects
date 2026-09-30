namespace Namaa.Domain.Entities;

public class ProjectFile
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long UploadedByUserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }

    public Project Project { get; set; } = null!;
    public User UploadedByUser { get; set; } = null!;
}
