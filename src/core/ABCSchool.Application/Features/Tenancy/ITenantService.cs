using System;

namespace ABCSchool.Application.Features.Tenancy;

public interface ITenantService
{
    Task<string> CreateTenantAsync(CreateTenantRequest request, CancellationToken ct);
    Task<string> ActivateAsync(string id);
    Task<string> DeactivateAsync(string id);
   
   Task<string> UpdateSubscriptionAsync(UpdateTenantSubscriptioRequest request);

   Task<List<TenantResponse>> GetTenantsAsync();

   Task<TenantResponse> GetTenantByIdAsync(string id);




}
