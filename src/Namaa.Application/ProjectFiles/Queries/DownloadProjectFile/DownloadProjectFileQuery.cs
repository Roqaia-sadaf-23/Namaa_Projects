using MediatR;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Queries.DownloadProjectFile;

public sealed record DownloadProjectFileQuery(
    long ProjectId,
    long Id) : IRequest<Result<ProjectFileDownload>>;

public sealed record ProjectFileDownload(
    Stream Content,
    string FileName,
    string ContentType);
