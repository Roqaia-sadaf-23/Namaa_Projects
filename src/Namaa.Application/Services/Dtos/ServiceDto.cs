using Namaa.Domain.Entities;

namespace Namaa.Application.Services.Dtos;

public sealed record ServiceDto(long Id, string Name, string? Category, string? Description, decimal DefaultPrice, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt)
{
    internal static ServiceDto FromEntity(Service e) => new(e.Id, e.Name, e.Category, e.Description, e.DefaultPrice, e.IsActive, e.CreatedAt, e.UpdatedAt);
}

