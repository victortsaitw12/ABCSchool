using System;

namespace ABCSchool.Application.Features.Identity.Roles;

public interface IRoleService
{
    Task<string> CreateAsync(CreateRoleRequest request);
    Task<string> UpdateAsync(UpdateRoleRequest request);
    Task<string> DeleteAsync(string id);
    Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request);
    Task<bool> DoesItExistAsync(string name);
    Task<List<RoleResponse>> GetAllAsync(CancellationToken ct);
    Task<RoleResponse> GetByIdAsync(string id, CancellationToken ct);
    Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken ct);
}
