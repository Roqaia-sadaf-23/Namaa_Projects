namespace Namaa.Api.Contracts.ProjectCorrespondences;

public sealed record CreateProjectCorrespondenceRequest(string CorrespondenceType, string Subject, DateTime CorrespondenceDate, string? ReferenceNumber, string? Notes);

