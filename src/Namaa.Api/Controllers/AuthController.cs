using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Authentication;
using Namaa.Api.Extensions;
using Namaa.Application.Authentication.Commands.Login;
using Namaa.Application.Authentication.Commands.Logout;
using Namaa.Application.Authentication.Commands.RefreshToken;

namespace Namaa.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken) =>
        (await sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken)).ToActionResult(this);

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken) =>
        (await sender.Send(
            new RefreshTokenCommand(request.RefreshToken),
            cancellationToken)).ToActionResult(this);

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        LogoutRequest request,
        CancellationToken cancellationToken) =>
        (await sender.Send(
            new LogoutCommand(request.RefreshToken),
            cancellationToken)).ToActionResult(this);
}
