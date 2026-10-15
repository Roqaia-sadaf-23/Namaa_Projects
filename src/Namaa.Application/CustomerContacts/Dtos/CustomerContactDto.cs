using Namaa.Domain.Entities;

namespace Namaa.Application.CustomerContacts.Dtos;

public sealed record CustomerContactDto(long Id, long CustomerId, string FullName, string? JobTitle, string? Email, string? PhoneNumber, bool IsPrimary, DateTime CreatedAt)
{
    internal static CustomerContactDto FromEntity(CustomerContact e) => new(e.Id, e.CustomerId, e.FullName, e.JobTitle, e.Email, e.PhoneNumber, e.IsPrimary, e.CreatedAt);
}

