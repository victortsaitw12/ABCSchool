using System;
using ABCSchool.Application.Features.Schools;
using ABCSchool.Domain.Entities;
using ABCSchool.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ABCSchool.Infrastructure.Schools;

public class SchoolService : ISchoolService
{
    private readonly ApplicationDbContext _context;

    public SchoolService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(School school)
    {
        await _context.Schools.AddAsync(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<int> DeleteAsync(School school)
    {
        _context.Schools.Remove(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<List<School>> GetAllAsync()
    {
        return await _context.Schools.ToListAsync();
    }

    public async Task<School> GetByIdAsync(int id)
    {
        return await _context.Schools
            .Where(school => school.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<School> GetByNameAsync(string name)
    {
        return await _context.Schools
            .Where(school => school.Name == name)
            .FirstOrDefaultAsync();
    }

    public async Task<int> UpdateAsync(School school)
    {
        _context.Schools.Update(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }
}
