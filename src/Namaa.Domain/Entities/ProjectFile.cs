namespace Namaa.Domain.Entities;

public class ProjectFile
{
    private ProjectFile()
    {
    }

    private ProjectFile(
        long projectId,
        long uploadedByUserId,
        string fileName,
        string storageKey,
        long fileSizeBytes,
        string? contentType,
        string? description,
        DateTime uploadedAt)
    {
        ProjectId = DomainGuard.Positive(projectId, nameof(projectId));
        UploadedByUserId = DomainGuard.Positive(uploadedByUserId, nameof(uploadedByUserId));
        FileName = DomainGuard.Required(fileName, nameof(fileName));
        StorageKey = DomainGuard.Required(storageKey, nameof(storageKey));
        FileSizeBytes = DomainGuard.Positive(fileSizeBytes, nameof(fileSizeBytes));
        ContentType = contentType;
        Description = description;
        UploadedAt = uploadedAt;
    }

    public long Id { get; private set; }
    public long ProjectId { get; private set; }
    public long UploadedByUserId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string StorageKey { get; private set; } = string.Empty;
    public string? ContentType { get; private set; }
    public long FileSizeBytes { get; private set; }
    public string? Description { get; private set; }
    public DateTime UploadedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public User UploadedByUser { get; private set; } = null!;

    public static ProjectFile Create(
        long projectId,
        long uploadedByUserId,
        string fileName,
        string storageKey,
        long fileSizeBytes,
        string? contentType = null,
        string? description = null,
        DateTime? uploadedAt = null) =>
        new(projectId, uploadedByUserId, fileName, storageKey, fileSizeBytes,
            contentType, description, uploadedAt ?? DateTime.UtcNow);

    public void UpdateDescription(string? description) => Description = description;
}
