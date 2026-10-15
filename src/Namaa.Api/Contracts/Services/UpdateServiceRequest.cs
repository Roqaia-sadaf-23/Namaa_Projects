namespace Namaa.Api.Contracts.Services;

public sealed record UpdateServiceRequest(string Name, decimal DefaultPrice, string? Category, string? Description, string RowVersion);

