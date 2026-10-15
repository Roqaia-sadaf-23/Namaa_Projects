namespace Namaa.Api.Contracts.ProjectCorrespondences;

public sealed record UpdateProjectCorrespondenceRequest(string CorrespondenceType, string Subject, DateTime CorrespondenceDate, string? ReferenceNumber, string? Notes);

