namespace Namaa.Api.Contracts.Services;

public sealed record CreateServiceRequest(string Name, decimal DefaultPrice, string? Category, string? Description);

