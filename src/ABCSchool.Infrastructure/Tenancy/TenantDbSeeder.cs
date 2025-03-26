using System;
using ABCSchool.Infrastructure.Contexts;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ABCSchool.Infrastructure.Tenancy;

/// <summary>
/// Handles the initialization and seeding of tenant-specific databases.
/// This seeder ensures that each tenant has their own isolated database with necessary initial data.
/// </summary>
public class TenantDbSeeder : ITenantDbSeeder
{
    private readonly TenantDbContext _tenantDbContext;
    private readonly IServiceProvider _serviceProvider;

    public TenantDbSeeder(TenantDbContext tenantDbContext, IServiceProvider serviceProvider)
    {
        _tenantDbContext = tenantDbContext;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Initializes the tenant database and seeds data for all tenants.
    /// This includes creating the root tenant and initializing application-specific data for each tenant.
    /// </summary>
    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        // First, ensure the tenant database is initialized with the root tenant
        await InitializeDatabaseWithTenantAsync(cancellationToken);

        // Then, for each tenant in the system, initialize their application-specific database
        foreach(var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
        {
            await InitializaApplicationDbForTenantAsync(tenant, cancellationToken);
        }
    }

    /// <summary>
    /// Creates the root tenant if it doesn't exist.
    /// The root tenant is a special tenant that has administrative privileges.
    /// </summary>
    private async Task InitializeDatabaseWithTenantAsync(CancellationToken cancellationToken)
    {
        if(await _tenantDbContext.TenantInfo.FindAsync([TenancyConstants.Root.Id], cancellationToken) is null)
        {
            // Create root tenant with predefined constants
            var rootTenant = new ABCSchoolTenantInfo
            {
                Id = TenancyConstants.Root.Id,
                Identifier = TenancyConstants.Root.Id,
                Name = TenancyConstants.Root.Name,
                Email = TenancyConstants.Root.Email,
                FirstName = TenancyConstants.FirstName,
                LastName = TenancyConstants.LastName,
                IsActive = true,
                ValidUpTo = DateTime.UtcNow.AddYears(2),
            };
            await _tenantDbContext.TenantInfo.AddAsync(rootTenant, cancellationToken);
            await _tenantDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Initializes the application-specific database for a given tenant.
    /// This includes setting up the tenant context and seeding tenant-specific data.
    /// </summary>
    private async Task InitializaApplicationDbForTenantAsync(ABCSchoolTenantInfo currentTenant, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        // Set the current tenant context for the application database operations
        _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
            {
                TenantInfo = currentTenant,
            };

        // Initialize the application database with tenant-specific data
        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }
}
