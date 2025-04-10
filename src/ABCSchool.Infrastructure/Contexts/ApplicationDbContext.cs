using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using ABCSchool.Infrastructure.Tenancy;
using ABCSchool.Domain.Entities;

namespace ABCSchool.Infrastructure.Contexts;

/// <summary>
/// Application-specific database context that handles all application-related entities.
/// This context is tenant-specific and inherits from BaseDbContext for common functionality.
/// </summary>
public class ApplicationDbContext: BaseDbContext
{
    // Constructor that takes tenant context accessor and database options
    public ApplicationDbContext(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor, 
        DbContextOptions<ApplicationDbContext> options
    ): base(tenantInfoContextAccessor, options)
    {

    }

    // DbSet for School entities with multi-tenancy support
    public DbSet<School> Schools => Set<School>();
}
