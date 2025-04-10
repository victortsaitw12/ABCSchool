using System;
using ABCSchool.Infrastructure.Constants;
using ABCSchool.Infrastructure.Identity;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ABCSchool.Infrastructure.Contexts;

public class ApplicationDbSeeder
{
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantInfoContextAccessor;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;

    public ApplicationDbSeeder(
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantInfoContextAccessor,
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext applicationDbContext)
    {
        _tenantInfoContextAccessor = tenantInfoContextAccessor;
        _roleManager = roleManager;
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
    }
    

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {

        if(_applicationDbContext.Database.GetMigrations().Any())
        {
            if((await _applicationDbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                await _applicationDbContext.Database.MigrateAsync(cancellationToken);
            }

            if(await _applicationDbContext.Database.CanConnectAsync(cancellationToken))
            {
                // Seeding data
                // Default roles > Assign permissions/calims
                await InitializeDefaultRolesAsync(cancellationToken);
                // Users > Assign roles
                await InitializaAdminUserAsync();
            }
        }
    }

    private async Task InitializeDefaultRolesAsync(CancellationToken cancellationToken)
    {
        foreach(var roleName in RoleConstants.DefaultRoles)
        {
            if(await _roleManager.Roles.SingleOrDefaultAsync(role => role.Name == roleName, cancellationToken) is not ApplicationRole incomingRole)
            {
                incomingRole = new ApplicationRole()
                {
                    Name = roleName,
                    Description = $"{roleName} Role"
                };
                await _roleManager.CreateAsync(incomingRole);
            }

            // Assign permissions/claims
            if(roleName == RoleConstants.Basic)
            {
                // Assign Basic permissions
                await AssignPermissionsToRole(SchoolPermissions.Basic, incomingRole, cancellationToken); 
            }
            else if(roleName == RoleConstants.Admin)
            {
                // Assign Admin permissions
                await AssignPermissionsToRole(SchoolPermissions.Admin, incomingRole, cancellationToken); 

                if(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Id == TenancyConstants.Root.Id)
                {
                    // Assign Root permissions
                    await AssignPermissionsToRole(SchoolPermissions.Root, incomingRole, cancellationToken); 
                }
            }
        }
    }

    private async Task AssignPermissionsToRole(
        IReadOnlyList<SchoolPermission> rolePermissions, ApplicationRole role, CancellationToken cancellationToken)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach(var rolePermission in rolePermissions)
        {
            if(!currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == rolePermission.Name))
            {
                await _applicationDbContext.RoleClaims.AddAsync(new ApplicationRoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = rolePermission.Name,
                    Description = rolePermission.Description,
                    Group = rolePermission.Group
                }, cancellationToken);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }


    private async Task InitializaAdminUserAsync()
    {
        if(string.IsNullOrEmpty(_tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email)) return;

        if(await _userManager.Users
            .SingleOrDefaultAsync(user => user.Email == _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email) 
            is not ApplicationUser incomingUser)
        {
            incomingUser = new ApplicationUser()
            {
                FirstName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.FirstName,
                LastName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.LastName,
                Email = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                UserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                NormalizedUserName = _tenantInfoContextAccessor.MultiTenantContext.TenantInfo.Email.ToUpperInvariant(),
                IsActive = true,
            };

            var passwordHash = new PasswordHasher<ApplicationUser>();
            incomingUser.PasswordHash = passwordHash.HashPassword(incomingUser, TenancyConstants.DefaultPassword);
            await _userManager.CreateAsync(incomingUser);
        }

        if(!await _userManager.IsInRoleAsync(incomingUser, RoleConstants.Admin))
        {
            await _userManager.AddToRoleAsync(incomingUser, RoleConstants.Admin);
        }
    }
}
