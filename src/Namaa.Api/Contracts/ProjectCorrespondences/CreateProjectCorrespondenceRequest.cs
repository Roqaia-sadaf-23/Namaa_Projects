namespace Namaa.Api.Contracts.ProjectCorrespondences;

public sealed record CreateProjectCorrespondenceRequest(string CorrespondenceType, string Subject, DateTime CorrespondenceDate, long CreatedByUserId, string? ReferenceNumber, string? Notes);

