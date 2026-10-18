using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Notifications;
using Namaa.Api.Extensions;
using Namaa.Application.Notifications.Commands.MarkNotificationRead;
using Namaa.Application.Notifications.Dtos;
using Namaa.Application.Notifications.Queries.GetNotificationById;
using Namaa.Application.Notifications.Queries.GetNotifications;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<NotificationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? isRead,
        CancellationToken cancellationToken) =>
        (await sender.Send(new GetNotificationsQuery(isRead), cancellationToken)).ToActionResult(this);

    [HttpGet("{id:long}")]
    [ProducesResponseType<NotificationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetNotificationByIdQuery(id), cancellationToken)).ToActionResult(this);

    [HttpPatch("{id:long}/read")]
    [ProducesResponseType<NotificationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead(
        long id,
        MarkNotificationReadRequest request,
        CancellationToken cancellationToken) =>
        (await sender.Send(
            new MarkNotificationReadCommand(id, request.IsRead),
            cancellationToken)).ToActionResult(this);
}
