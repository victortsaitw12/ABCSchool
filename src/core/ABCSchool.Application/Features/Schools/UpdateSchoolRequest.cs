using System;

namespace ABCSchool.Application.Features.Schools;

public class UpdateSchoolRequest
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime EstablishedDate { get; set; }
}
