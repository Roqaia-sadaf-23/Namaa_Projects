using FluentValidation;

namespace Namaa.Application.ProjectCorrespondences.Commands.CreateProjectCorrespondence;

public sealed class CreateProjectCorrespondenceCommandValidator : AbstractValidator<CreateProjectCorrespondenceCommand>
{
    public CreateProjectCorrespondenceCommandValidator()
    {
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.CorrespondenceType).Must(x => x is "Incoming" or "Outgoing");
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(300);
        RuleFor(x => x.ReferenceNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}

