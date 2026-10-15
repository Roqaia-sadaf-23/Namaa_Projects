using FluentValidation;

namespace Namaa.Application.Branches.Commands.UpdateBranch;

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator() { RuleFor(x => x.Id).GreaterThan((long)0); RuleFor(x => x.Name).NotEmpty().MaximumLength(200); RuleFor(x => x.Address).MaximumLength(500); RuleFor(x => x.City).MaximumLength(100); }
}

