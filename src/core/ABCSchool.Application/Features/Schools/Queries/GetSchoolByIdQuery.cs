using System;
using MediatR;
using ABCSchool.Application.Wrappers;
using Mapster;
namespace ABCSchool.Application.Features.Schools.Queries;

public class GetSchoolByIdQuery: IRequest<IResponseWrapper>
{
    public int SchoolId { get; set; }
}

public class GetSchoolByIdQueryHandler: IRequestHandler<GetSchoolByIdQuery, IResponseWrapper>
{
    private readonly ISchoolService _schoolService;

    public GetSchoolByIdQueryHandler(ISchoolService schoolService)
    {
        _schoolService = schoolService;
    }

    public async Task<IResponseWrapper> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
    {
        var schoolInDb = await _schoolService.GetByIdAsync(request.SchoolId);
        if (schoolInDb is not null)
        {
            return await ResponseWrapper<SchoolResponse>.SuccessAsync(data: schoolInDb.Adapt<SchoolResponse>());
        }
        return await ResponseWrapper<SchoolResponse>.FailAsync("School does not exist.");
    }
}