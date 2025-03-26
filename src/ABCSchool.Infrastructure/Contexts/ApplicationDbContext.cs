using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using ABCSchool.Infrastructure.Tenancy;
using ABCSchool.Domain.Entities;

namespace ABCSchool.Infrastructure.Contexts;

public class ApplicationDbContext: BaseDbContext
{
    public ApplicationDbContext(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor, 
        DbContextOptions<ApplicationDbContext> options
    ): base(tenantInfoContextAccessor, options)
    {

    }

    public DbSet<School> Schools => Set<School>();
}
