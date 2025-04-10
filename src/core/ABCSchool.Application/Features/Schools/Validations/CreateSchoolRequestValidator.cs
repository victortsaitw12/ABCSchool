using System;
using FluentValidation;

namespace ABCSchool.Application.Features.Schools.Validations;

public class CreateSchoolRequestValidator: AbstractValidator<CreateSchoolRequest>
{
    public CreateSchoolRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
                .WithMessage("School name is required.")
            .MaximumLength(60);

        RuleFor(request => request.EstablishedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date established cannot be future date.");
    }
}
