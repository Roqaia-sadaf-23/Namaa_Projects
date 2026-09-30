namespace Namaa.Domain.Entities;

public class ProjectCorrespondence
{
    private ProjectCorrespondence()
    {
    }

    private ProjectCorrespondence(
        long projectId,
        string correspondenceType,
        string subject,
        DateTime correspondenceDate,
        long createdByUserId,
        string? referenceNumber,
        string? notes)
    {
        ProjectId = DomainGuard.Positive(projectId, nameof(projectId));
        CorrespondenceType = DomainGuard.OneOf(
            correspondenceType, nameof(correspondenceType), "Incoming", "Outgoing");
        Subject = DomainGuard.Required(subject, nameof(subject));
        CorrespondenceDate = correspondenceDate;
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        ReferenceNumber = referenceNumber;
        Notes = notes;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public long ProjectId { get; private set; }
    public string CorrespondenceType { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string? ReferenceNumber { get; private set; }
    public DateTime CorrespondenceDate { get; private set; }
    public string? Notes { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public User CreatedByUser { get; private set; } = null!;

    public static ProjectCorrespondence Create(
        long projectId,
        string correspondenceType,
        string subject,
        DateTime correspondenceDate,
        long createdByUserId,
        string? referenceNumber = null,
        string? notes = null) =>
        new(projectId, correspondenceType, subject, correspondenceDate,
            createdByUserId, referenceNumber, notes);

    public void Update(
        string correspondenceType,
        string subject,
        DateTime correspondenceDate,
        string? referenceNumber = null,
        string? notes = null)
    {
        CorrespondenceType = DomainGuard.OneOf(
            correspondenceType, nameof(correspondenceType), "Incoming", "Outgoing");
        Subject = DomainGuard.Required(subject, nameof(subject));
        CorrespondenceDate = correspondenceDate;
        ReferenceNumber = referenceNumber;
        Notes = notes;
    }
}
