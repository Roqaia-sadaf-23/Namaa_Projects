using Namaa.Domain.Entities;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Application.Suppliers.Dtos;

public sealed record SupplierDto(long Id, string SupplierCode, string Name, string? PhoneNumber, string? Email, string? Address, string? City, string? TaxNumber, string? CommercialRegister, string? Notes, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt, string RowVersion)
{
    internal static SupplierDto FromEntity(Supplier e) => new(e.Id, e.SupplierCode, e.Name, e.PhoneNumber, e.Email, e.Address, e.City, e.TaxNumber, e.CommercialRegister, e.Notes, e.IsActive, e.CreatedAt, e.UpdatedAt, ConcurrencyToken.Encode(e.RowVersion));
}

