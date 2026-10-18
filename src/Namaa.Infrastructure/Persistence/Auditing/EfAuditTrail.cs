using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Auditing;
using Namaa.Application.Common.Auditing;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Auditing;

public sealed class EfAuditTrail(NamaaDbContext dbContext) : IAuditTrail
{
    public async Task<TResponse> ExecuteAsync<TResponse>(
        Func<Task<TResponse>> operation,
        Func<TResponse, AuditRecord?> auditRecordFactory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ArgumentNullException.ThrowIfNull(auditRecordFactory);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var response = await operation();
        var record = auditRecordFactory(response);

        if (record is null)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            return response;
        }

        dbContext.AuditLogs.Add(AuditLog.Create(
            record.Action,
            record.EntityType,
            record.ActorUserId,
            record.EntityId,
            newValues: record.NewValues));

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}
