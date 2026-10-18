using Namaa.Application.Common.Auditing;

namespace Namaa.Application.Abstractions.Auditing;

public interface IAuditTrail
{
    Task<TResponse> ExecuteAsync<TResponse>(
        Func<Task<TResponse>> operation,
        Func<TResponse, AuditRecord?> auditRecordFactory,
        CancellationToken cancellationToken = default);
}
