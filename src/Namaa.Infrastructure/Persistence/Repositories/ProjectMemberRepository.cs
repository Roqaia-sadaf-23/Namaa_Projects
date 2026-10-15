using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class ProjectMemberRepository(NamaaDbContext dbContext) : IProjectMemberRepository
{
    public Task<bool> ExistsAsync(
        long projectId,
        long userId,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(
                member => member.ProjectId == projectId && member.UserId == userId,
                cancellationToken);

    public Task<ProjectMember?> FindByIdAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(
                member => member.ProjectId == projectId && member.Id == id,
                cancellationToken);

    public Task<ProjectMember?> FindForUpdateAsync(
        long projectId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectMembers
            .SingleOrDefaultAsync(
                member => member.ProjectId == projectId && member.Id == id,
                cancellationToken);

    public async Task<IReadOnlyList<ProjectMember>> GetByProjectIdAsync(
        long projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectMembers
            .AsNoTracking()
            .Where(member => member.ProjectId == projectId)
            .OrderBy(member => member.JoinedAt)
            .ThenBy(member => member.Id)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        ProjectMember member,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectMembers.AddAsync(member, cancellationToken);
}
