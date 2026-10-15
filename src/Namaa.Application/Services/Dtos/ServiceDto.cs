using Namaa.Domain.Entities;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Application.Services.Dtos;

public sealed record ServiceDto(long Id, string Name, string? Category, string? Description, decimal DefaultPrice, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt, string RowVersion)
{
    internal static ServiceDto FromEntity(Service e) => new(e.Id, e.Name, e.Category, e.Description, e.DefaultPrice, e.IsActive, e.CreatedAt, e.UpdatedAt, ConcurrencyToken.Encode(e.RowVersion));
}

