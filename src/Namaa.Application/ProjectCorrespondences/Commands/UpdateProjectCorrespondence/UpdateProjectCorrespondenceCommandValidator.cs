using FluentValidation;

namespace Namaa.Application.ProjectCorrespondences.Commands.UpdateProjectCorrespondence;

public sealed class UpdateProjectCorrespondenceCommandValidator : AbstractValidator<UpdateProjectCorrespondenceCommand>
{
    public UpdateProjectCorrespondenceCommandValidator()
    {
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CorrespondenceType).Must(x => x is "Incoming" or "Outgoing");
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

