namespace Namaa.Api.Contracts.Suppliers;

public sealed record UpdateSupplierRequest(string Name, string? PhoneNumber, string? Email, string? Address, string? City, string? TaxNumber, string? CommercialRegister, string? Notes, string RowVersion);

