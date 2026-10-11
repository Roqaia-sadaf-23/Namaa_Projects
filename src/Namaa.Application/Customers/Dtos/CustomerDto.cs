using Namaa.Domain.Entities;

namespace Namaa.Application.Customers.Dtos;

public sealed record CustomerDto(
    long Id,
    string CustomerCode,
    string Name,
    string CustomerType,
    string? Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? TaxNumber,
    string? CommercialRegister,
    string? Source,
    string? Notes,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    internal static CustomerDto FromEntity(Customer customer) =>
        new(
            customer.Id,
            customer.CustomerCode,
            customer.Name,
            customer.CustomerType,
            customer.Email,
            customer.PhoneNumber,
            customer.Address,
            customer.City,
            customer.TaxNumber,
            customer.CommercialRegister,
            customer.Source,
            customer.Notes,
            customer.IsActive,
            customer.CreatedAt,
            customer.UpdatedAt);
}
