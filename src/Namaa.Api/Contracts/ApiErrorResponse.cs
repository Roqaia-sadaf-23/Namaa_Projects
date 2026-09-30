namespace Namaa.Api.Contracts;

public sealed record ApiErrorResponse(string Code, string Message, string Type);
