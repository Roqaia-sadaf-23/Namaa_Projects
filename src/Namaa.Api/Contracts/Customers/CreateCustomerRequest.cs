namespace Namaa.Api.Contracts.Customers;

public sealed record CreateCustomerRequest(
    string CustomerCode,
    string Name,
    string CustomerType = "Company");
