using MediatR;
using Namaa.Application.Authentication.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Authentication.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthTokenDto>>;
