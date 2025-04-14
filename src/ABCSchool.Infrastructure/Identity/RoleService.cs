using System;
using ABCSchool.Application.Exceptions;
using ABCSchool.Application.Features.Identity.Roles;
using ABCSchool.Infrastructure.Constants;
using ABCSchool.Infrastructure.Contexts;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace ABCSchool.Infrastructure.Identity;

public class RoleService : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor;
    public RoleService(
        RoleManager<ApplicationRole> roleManager, 
        UserManager<ApplicationUser> userManager, 
        ApplicationDbContext context, 
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _tenantInfoContextAccessor = tenantInfoContextAccessor;
    }
    public async Task<string> CreateAsync(CreateRoleRequest request)
    {
        var newRole = new ApplicationRole()
        {
            Name = request.Name,
            Description = request.Description,
        };

        var result = await _roleManager.CreateAsync(newRole);
        if (!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return newRole.Name;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var roleInDb = await _roleManager.FindByIdAsync(id)
            ?? throw new NotFoundException(["Role does not exist"]);

        if(RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Not allowed to delete '{roleInDb.Name}' role."]);
        }
        
        if((await _userManager.GetUsersInRoleAsync(roleInDb.Name)).Count > 0)
        {
            throw new ConflictException([$"Not allowed to delete '{roleInDb.Name}' role as is currently assigned to users."]);
        }

        var result = await _roleManager.DeleteAsync(roleInDb);
        if(!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return roleInDb.Name;
    }

    public async Task<bool> DoesItExistAsync(string name)
    {
        return await _roleManager.RoleExistsAsync(name);
    }

    public async Task<List<RoleResponse>> GetAllAsync(CancellationToken ct)
    {
        var rolesInDb = await _roleManager.Roles.ToListAsync(ct);
        return rolesInDb.Adapt<List<RoleResponse>>();
    }

    public async Task<RoleResponse> GetByIdAsync(string id, CancellationToken ct)
    {
        var roleInDb = await _context.Roles.FirstOrDefaultAsync(role => role.Id == id, ct)
            ?? throw new NotFoundException(["Role does not exist"]);

        return roleInDb.Adapt<RoleResponse>();
    }

    public async Task<RoleResponse> GetRoleWithPermissionsAsync(string id, CancellationToken ct)
    {
        var role = await GetByIdAsync(id, ct);

        role.Permissions = await _context.RoleClaims
            .Where(rc => rc.RoleId == id && rc.ClaimType == ClaimConstants.Permission)
            .Select(rc => rc.ClaimValue)
            .ToListAsync(ct);

        return role;
    }

    public async Task<string> UpdateAsync(UpdateRoleRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.Id)
            ?? throw new NotFoundException(["Role does not exist"]);

        if(RoleConstants.IsDefaultRole(roleInDb.Name))
        {
            throw new ConflictException([$"Changes not allowed on system role '{roleInDb.Name}'."]);
        }

        roleInDb.Name = request.Name;
        roleInDb.Description = request.Description;
        roleInDb.NormalizedName = request.Name.ToUpperInvariant();

        var result = await _roleManager.UpdateAsync(roleInDb);
        if(!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return roleInDb.Name;
    }

    public async Task<string> UpdatePermissionsAsync(UpdateRolePermissionsRequest request)
    {
        var roleInDb = await _roleManager.FindByIdAsync(request.RoleId)
            ?? throw new NotFoundException(["Role does not exist"]);

        if(roleInDb.Name == RoleConstants.Admin)
        {
            throw new ConflictException([$"Not allowed to change permissions for '{roleInDb.Name}' role."]);
        }

        if(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
        {
            request.NewPermissions.RemoveAll(p => p.StartsWith("Permission.Tenants."));
        }

        // Drop and lift
        var currentClaims = await _roleManager.GetClaimsAsync(roleInDb);

        foreach(var claim in currentClaims.Where(c => !request.NewPermissions.Any(p => p == c.Value)))
        {
            var result = await _roleManager.RemoveClaimAsync(roleInDb, claim);

            if(!result.Succeeded)
            {
                throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
            }
        }

        foreach(var newPermission in request.NewPermissions.Where(p => !currentClaims.Any(c => c.Value == p)))
        {
            await _context
                .RoleClaims
                .AddAsync(new ApplicationRoleClaim
                {
                    RoleId = roleInDb.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = newPermission,
                    Description = "",
                    Group = "",
                });
        }
        await _context.SaveChangesAsync();

        return "Permissions Updated Successfully";
    }


}
