using System;
using ABCSchool.Application.Features.Tenancy;
using ABCSchool.Infrastructure.Contexts;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace ABCSchool.Infrastructure.Tenancy;

public class TenantService : ITenantService
{
    private readonly IMultiTenantStore<ABCSchoolTenantInfo> _tenantStore;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly IServiceProvider _serviceProvider;
    public TenantService(
        IMultiTenantStore<ABCSchoolTenantInfo> tenantStore,
        ApplicationDbSeeder dbSeeder,
        IServiceProvider serviceProvider
    )
    {
        _tenantStore = tenantStore;
        _dbSeeder = dbSeeder;
        _serviceProvider = serviceProvider;
    }
    public async Task<string> ActivateAsync(string id)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(id);
        tenantInDb.IsActive = true;

        await _tenantStore.TryUpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }

    public async Task<string> CreateTenantAsync(
        CreateTenantRequest request, CancellationToken ct)
    {
        var newTenant = new ABCSchoolTenantInfo
        {
            Id = request.Identifier,
            Identifier = request.Identifier,
            Name = request.Name,
            IsActive = request.IsActive,
            ConnectionString = request.ConnectionString,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ValidUpTo = request.ValidUpTo,
        };

        await _tenantStore.TryAddAsync(newTenant);

        // seedin tenant data
        using var scope = _serviceProvider.CreateScope();

        _serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
            .MultiTenantContext = new MultiTenantContext<ABCSchoolTenantInfo>()
            {
                TenantInfo = newTenant,
            };

        await scope.ServiceProvider.GetRequiredService<ApplicationDbSeeder>()
            .InitializeDatabaseAsync(ct);

        return newTenant.Identifier;
    }

    public async Task<string> DeactivateAsync(string id)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(id);
        tenantInDb.IsActive = false;

        await _tenantStore.TryUpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }

    public async Task<TenantResponse> GetTenantByIdAsync(string id)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(id);
        return tenantInDb.Adapt<TenantResponse>();
    }

    public async Task<List<TenantResponse>> GetTenantsAsync()
    {
        var tenantsInDb = await _tenantStore.GetAllAsync();
        return tenantsInDb.Adapt<List<TenantResponse>>();
    }

    public async Task<string> UpdateSubscriptionAsync(
        UpdateTenantSubscriptioRequest request)
    {
        var tenantInDb = await _tenantStore.TryGetAsync(request.TenantId);
        tenantInDb.ValidUpTo = request.NewExpiryDate;

        await _tenantStore.TryUpdateAsync(tenantInDb);
        return tenantInDb.Identifier;
    }
}
