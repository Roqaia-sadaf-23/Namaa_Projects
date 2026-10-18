using Namaa.Application.Abstractions.Notifications;

namespace Namaa.Application.Notifications.Events;

public static class NotificationTypes
{
    public const string ProjectTaskAssigned = "Project.TaskAssigned";
    public const string ProjectMemberAdded = "Project.MemberAdded";
    public const string InvoiceIssued = "Invoice.Issued";
}

internal static class NotificationCatalog
{
    public static NotificationWriteModel For(ProjectTaskAssignedNotificationEvent notification) =>
        new(
            notification.RecipientUserId,
            NotificationTypes.ProjectTaskAssigned,
            notification.TaskId,
            "Project task assigned",
            $"You were assigned the project task '{notification.TaskTitle}'.");

    public static NotificationWriteModel For(ProjectMemberAddedNotificationEvent notification) =>
        new(
            notification.RecipientUserId,
            NotificationTypes.ProjectMemberAdded,
            notification.ProjectMemberId,
            "Project membership added",
            $"You were added to project {notification.ProjectId}.");

    public static NotificationWriteModel For(InvoiceIssuedNotificationEvent notification) =>
        new(
            notification.RecipientUserId,
            NotificationTypes.InvoiceIssued,
            notification.InvoiceId,
            "Invoice issued",
            $"Invoice '{notification.InvoiceNumber}' was issued.");
}
