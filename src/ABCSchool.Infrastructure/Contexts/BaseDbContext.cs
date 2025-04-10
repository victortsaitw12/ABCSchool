using System;
using System.Reflection;
using ABCSchool.Infrastructure.Identity;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ABCSchool.Infrastructure.Contexts;

/// <summary>
/// Base database context that provides common functionality for all database contexts in the application.
/// This includes tenant-specific filtering and common entity configurations.
/// </summary>
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
    // Override TenantInfo to use ABCSchoolTenantInfo type
    private new ABCSchoolTenantInfo TenantInfo {get; set;}

    // Constructor that takes tenant context accessor and database options
    public BaseDbContext(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor, 
        DbContextOptions options
    ): base(tenantInfoContextAccessor, options)
    {
        TenantInfo = tenantInfoContextAccessor.MultiTenantContext.TenantInfo;
    }

    // Configure database connection based on tenant
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Use tenant-specific connection string if available
        if(!string.IsNullOrEmpty(TenantInfo?.ConnectionString))
        {
            optionsBuilder.UseSqlServer(TenantInfo.ConnectionString, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
        }
    }

    // Apply entity configurations from the current assembly
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
