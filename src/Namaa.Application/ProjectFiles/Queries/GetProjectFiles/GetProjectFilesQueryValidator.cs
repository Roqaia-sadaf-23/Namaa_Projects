using FluentValidation;

namespace Namaa.Application.ProjectFiles.Queries.GetProjectFiles;

public sealed class GetProjectFilesQueryValidator : AbstractValidator<GetProjectFilesQuery>
{
    public GetProjectFilesQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
    }
}
