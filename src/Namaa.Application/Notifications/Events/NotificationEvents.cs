using MediatR;

namespace Namaa.Application.Notifications.Events;

public sealed record ProjectTaskAssignedNotificationEvent(
    long RecipientUserId,
    long TaskId,
    string TaskTitle) : INotification;

public sealed record ProjectMemberAddedNotificationEvent(
    long RecipientUserId,
    long ProjectMemberId,
    long ProjectId) : INotification;

public sealed record InvoiceIssuedNotificationEvent(
    long RecipientUserId,
    long InvoiceId,
    string InvoiceNumber) : INotification;
