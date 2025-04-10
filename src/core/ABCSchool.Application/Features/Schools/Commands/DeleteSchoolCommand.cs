using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Schools.Commands;

public class DeleteSchoolCommand: IRequest<IResponseWrapper>
{
    public int schoolId { get; set; }
}

public class DeleteSchoolCommandHandler: IRequestHandler<DeleteSchoolCommand, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public DeleteSchoolCommandHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetByIdAsync(request.schoolId);
        if (schoolInDb is not null)
        {
            var deletedSchoolId = await _schoolService.DeleteAsync(schoolInDb);
            return await ResponseWrapper<int>.SuccessAsync(data: deletedSchoolId, "School deleted successfully.");
        }
        return await ResponseWrapper<int>.FailAsync("School does not exist.");
    }
}
