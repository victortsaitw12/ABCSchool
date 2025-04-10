using System;
using ABCSchool.Domain.Entities;

namespace ABCSchool.Application.Features.Schools;

public interface ISchoolService
{
    Task<int> CreateAsync(School school);
    Task<int> UpdateAsync(School school);
    Task<int> DeleteAsync(School school);
    Task<School> GetByIdAsync(int id);
    Task<List<School>> GetAllAsync();
    Task<School> GetByNameAsync(string name);
}
