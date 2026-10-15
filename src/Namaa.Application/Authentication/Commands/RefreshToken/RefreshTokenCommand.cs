using MediatR;
using Namaa.Application.Authentication.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Authentication.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokenDto>>;
