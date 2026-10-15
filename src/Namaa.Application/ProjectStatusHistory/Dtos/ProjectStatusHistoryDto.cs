namespace Namaa.Application.ProjectStatusHistory.Dtos;

public sealed record ProjectStatusHistoryDto(long Id, long ProjectId, string? OldStatus, string NewStatus, string? Reason, long ChangedByUserId, DateTime ChangedAt)
{
    internal static ProjectStatusHistoryDto FromEntity(Namaa.Domain.Entities.ProjectStatusHistory entity) => new(entity.Id, entity.ProjectId, entity.OldStatus, entity.NewStatus, entity.Reason, entity.ChangedByUserId, entity.ChangedAt);
}

