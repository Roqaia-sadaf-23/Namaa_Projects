using Namaa.Domain.Common.Results;

namespace Namaa.Domain.Common.Errors;

public static class ProjectErrors
{
    public static readonly Error NotFound = new(
        "Projects.NotFound",
        "The requested project was not found.",
        ErrorType.NotFound);

    public static readonly Error DuplicateProjectCode = new(
        "Projects.DuplicateProjectCode",
        "A project with the same project code already exists.",
        ErrorType.Conflict);

    public static readonly Error CustomerNotFound = new(
        "Projects.CustomerNotFound",
        "The customer specified for the project was not found.",
        ErrorType.NotFound);

    public static readonly Error CreatedByUserNotFound = new(
        "Projects.CreatedByUserNotFound",
        "The user creating the project was not found.",
        ErrorType.NotFound);

    public static readonly Error SourceQuotationNotFound = new(
        "Projects.SourceQuotationNotFound",
        "The source quotation specified for the project was not found.",
        ErrorType.NotFound);

    public static readonly Error SourceQuotationAlreadyAssigned = new(
        "Projects.SourceQuotationAlreadyAssigned",
        "The source quotation is already assigned to another project.",
        ErrorType.Conflict);

    public static readonly Error ProjectManagerNotFound = new(
        "Projects.ProjectManagerNotFound",
        "The project manager was not found.",
        ErrorType.NotFound);
}
