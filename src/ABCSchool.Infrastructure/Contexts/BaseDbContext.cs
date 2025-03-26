using System;
using System.Reflection;
using ABCSchool.Infrastructure.Identity;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ABCSchool.Infrastructure.Contexts;

public abstract class BaseDbContext:
    MultiTenantIdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        ApplicationRoleClaim,
        IdentityUserToken<string>
    >
{
    private new ABCSchoolTenantInfo TenantInfo {get; set;}
    public BaseDbContext(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor, 
        DbContextOptions options
    ): base(tenantInfoContextAccessor, options)
    {
        TenantInfo = tenantInfoContextAccessor.MultiTenantContext.TenantInfo;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if(!string.IsNullOrEmpty(TenantInfo?.ConnectionString))
        {
            optionsBuilder.UseSqlServer(TenantInfo.ConnectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
