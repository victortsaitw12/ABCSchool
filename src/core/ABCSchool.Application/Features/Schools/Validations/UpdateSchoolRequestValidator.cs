using System;
using ABCSchool.Domain.Entities;
using FluentValidation;

namespace ABCSchool.Application.Features.Schools.Validations;

public class UpdateSchoolRequestValidator: AbstractValidator<UpdateSchoolRequest>
{
    public UpdateSchoolRequestValidator(ISchoolService schoolService)
    {
        RuleFor(request => request.Id)
            .NotEmpty()
            .MustAsync(async (id, ct) => await schoolService.GetByIdAsync(id) is School schoolInDb && schoolInDb.Id == id)
            .WithMessage("School does not exist.");

        RuleFor(request => request.Name)
            .NotEmpty()
                .WithMessage("School name is required.")
            .MaximumLength(60);

        RuleFor(request => request.EstablishedDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Date established cannot be future date.");
    }
}
