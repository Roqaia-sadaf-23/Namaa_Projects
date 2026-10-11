using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<bool> ExistsByCodeAsync(
        string projectCode,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsBySourceQuotationIdAsync(
        long sourceQuotationId,
        CancellationToken cancellationToken = default);

    Task<Project?> FindByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Project?> FindForUpdateAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default);
}
