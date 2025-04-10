using System;
using ABCSchool.Application.Features.Schools.Commands;
using FluentValidation;

namespace ABCSchool.Application.Features.Schools.Validations;

public class CreateSchoolCommandValidator: AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(command => command.CreateSchool)
            .SetValidator(new CreateSchoolRequestValidator());
    }
}
