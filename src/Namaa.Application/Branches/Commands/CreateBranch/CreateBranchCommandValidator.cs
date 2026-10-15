using FluentValidation;

namespace Namaa.Application.Branches.Commands.CreateBranch;

public sealed class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator() { RuleFor(x => x.BranchCode).NotEmpty().MaximumLength(20); RuleFor(x => x.Name).NotEmpty().MaximumLength(200); RuleFor(x => x.Address).MaximumLength(500); RuleFor(x => x.City).MaximumLength(100); }
}

