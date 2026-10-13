using MediatR;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Queries.GetProjectFiles;

public sealed record GetProjectFilesQuery(
    long ProjectId) : IRequest<Result<IReadOnlyList<ProjectFileDto>>>;
