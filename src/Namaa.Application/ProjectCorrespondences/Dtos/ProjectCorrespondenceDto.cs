using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectCorrespondences.Dtos;

public sealed record ProjectCorrespondenceDto(long Id, long ProjectId, string CorrespondenceType, string Subject, string? ReferenceNumber, DateTime CorrespondenceDate, string? Notes, long CreatedByUserId, DateTime CreatedAt)
{
    internal static ProjectCorrespondenceDto FromEntity(ProjectCorrespondence entity) =>
        new(entity.Id, entity.ProjectId, entity.CorrespondenceType, entity.Subject, entity.ReferenceNumber, entity.CorrespondenceDate, entity.Notes, entity.CreatedByUserId, entity.CreatedAt);
}

