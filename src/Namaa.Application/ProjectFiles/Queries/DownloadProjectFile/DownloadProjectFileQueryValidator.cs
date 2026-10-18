using FluentValidation;

namespace Namaa.Application.ProjectFiles.Queries.DownloadProjectFile;

public sealed class DownloadProjectFileQueryValidator : AbstractValidator<DownloadProjectFileQuery>
{
    public DownloadProjectFileQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
