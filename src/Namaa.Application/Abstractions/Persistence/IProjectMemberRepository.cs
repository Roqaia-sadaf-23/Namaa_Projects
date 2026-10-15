using Namaa.Domain.Entities;

namespace Namaa.Application.Abstractions.Persistence;

public interface IProjectMemberRepository
{
    Task<bool> ExistsAsync(
        long projectId,
        long userId,
        CancellationToken cancellationToken = default);

    Task<ProjectMember?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<ProjectMember?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectMember>> GetByProjectIdAsync(
        long projectId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        ProjectMember member,
        CancellationToken cancellationToken = default);
}
