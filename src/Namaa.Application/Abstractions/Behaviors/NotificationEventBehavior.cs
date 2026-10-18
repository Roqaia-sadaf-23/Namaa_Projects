using MediatR;
using Namaa.Application.Invoices.Commands.IssueInvoice;
using Namaa.Application.Invoices.Dtos;
using Namaa.Application.Notifications.Events;
using Namaa.Application.ProjectMembers.Commands.AddProjectMember;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Application.ProjectTasks.Commands.CreateProjectTask;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Abstractions.Behaviors;

public sealed class NotificationEventBehavior<TRequest, TResponse>(IPublisher publisher)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        foreach (var notification in CreateNotifications(request, response))
            await publisher.Publish(notification, cancellationToken);

        return response;
    }

    private static IReadOnlyList<INotification> CreateNotifications(
        TRequest request,
        TResponse response) =>
        (request, response) switch
        {
            (CreateProjectTaskCommand, Result<ProjectTaskDto> result)
                when result.IsSuccess && result.Value.AssignedToUserId is long recipientUserId =>
                [new ProjectTaskAssignedNotificationEvent(
                    recipientUserId,
                    result.Value.Id,
                    result.Value.Title)],

            (AddProjectMemberCommand, Result<ProjectMemberDto> result) when result.IsSuccess =>
                [new ProjectMemberAddedNotificationEvent(
                    result.Value.UserId,
                    result.Value.Id,
                    result.Value.ProjectId)],

            (IssueInvoiceCommand, Result<InvoiceDto> result) when result.IsSuccess =>
                [new InvoiceIssuedNotificationEvent(
                    result.Value.CreatedByUserId,
                    result.Value.Id,
                    result.Value.InvoiceNumber)],

            _ => []
        };
}
