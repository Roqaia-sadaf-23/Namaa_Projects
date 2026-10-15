using MediatR;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Authentication.Commands.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
