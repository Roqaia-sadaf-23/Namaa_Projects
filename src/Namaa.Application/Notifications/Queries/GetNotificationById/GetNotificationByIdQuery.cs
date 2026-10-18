using MediatR;
using Namaa.Application.Notifications.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Notifications.Queries.GetNotificationById;

public sealed record GetNotificationByIdQuery(long Id) : IRequest<Result<NotificationDto>>;
