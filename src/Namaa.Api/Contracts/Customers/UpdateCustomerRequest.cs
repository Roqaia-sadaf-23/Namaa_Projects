namespace Namaa.Api.Contracts.Customers;

public sealed record UpdateCustomerRequest(
    string Name,
    string CustomerType,
    string? Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? TaxNumber,
    string? CommercialRegister,
    string? Source,
    string? Notes);
