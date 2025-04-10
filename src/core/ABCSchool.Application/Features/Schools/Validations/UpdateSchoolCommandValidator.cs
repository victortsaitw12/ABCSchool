using System;
using ABCSchool.Application.Features.Schools.Commands;
using FluentValidation;

namespace ABCSchool.Application.Features.Schools.Validations;

public class UpdateSchoolCommandValidator: AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator(ISchoolService schoolService)
    {
        RuleFor(command => command.UpdateSchool)
            .SetValidator(new UpdateSchoolRequestValidator(schoolService));
    }
}
