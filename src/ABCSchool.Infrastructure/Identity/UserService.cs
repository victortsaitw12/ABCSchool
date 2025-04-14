using System;
using ABCSchool.Application.Exceptions;
using ABCSchool.Application.Features.Identity.Users;
using ABCSchool.Infrastructure.Constants;
using ABCSchool.Infrastructure.Contexts;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;
namespace ABCSchool.Infrastructure.Identity;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _tenantContextAccessor = tenantContextAccessor;
    }
    public async Task<string> ActivateOrDeactivateAsync(string userId, bool activation)
    {
        var userInDb = await GetUserAsync(userId);

        userInDb.IsActive = activation;
        var result = await _userManager.UpdateAsync(userInDb);
        if(!result.Succeeded)
        {
            throw new IdentityException(IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }

        return userId;
    }

    public async Task<string> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        var userInDb = await GetUserAsync(userId);
        if(await _userManager.IsInRoleAsync(userInDb, RoleConstants.Admin)
            && request.UserRoles.Any(ur => !ur.IsAssigned && ur.Name == RoleConstants.Admin))
        {
            var adminUsersCount = (await _userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count;
            if(userInDb.Email == TenancyConstants.Root.Email)
            {
                if(_tenantContextAccessor.MultiTenantContext.TenantInfo.Id != TenancyConstants.Root.Id)
                {
                    throw new ConflictException(["Not allowed to remove admin role from Root Tenant user"]);
                }
            }
            else if(adminUsersCount <= 2)
            {
                throw new ConflictException(["Not allowed. Tenant should have at least two admin users"]);
            }
        }
        
        foreach(var userRole in request.UserRoles)
        {
            if(userRole.IsAssigned)
            {
                if(!await _userManager.IsInRoleAsync(userInDb, userRole.Name))
                {
                    await _userManager.AddToRoleAsync(userInDb, userRole.Name);
                }
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(userInDb, userRole.Name);
            }
        }

        return userId;
    }

    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userInDb = await GetUserAsync(request.UserId);
        if(request.NewPassword != request.ConfirmNewPassword)
        {
            throw new ConflictException(["New password and confirm password do not match"]);
        }
        var result = await _userManager.ChangePasswordAsync(
            userInDb, request.CurrentPassword, request.NewPassword);
        
        if(!result.Succeeded)
        {
            throw new IdentityException(
                IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return userInDb.Id;
    }

    public async Task<string> CreateAsync(CreateUserRequest request)
    {
        if(request.Password != request.ConfirmPassword)
        {
            throw new ConflictException(["Password and confirm password do not match"]);
        }
        if(await IsEmailTakenAsync(request.Email))
        {
            throw new ConflictException(["Email is already taken"]);
        }
        var newUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            UserName = request.Email,
            EmailConfirmed = true,
        };
        var result = await _userManager.CreateAsync(newUser, request.Password);
        if(!result.Succeeded)
        {
            throw new IdentityException(
                IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return newUser.Id;
    }

    public async Task<string> DeleteAsync(string userId)
    {
        var userInDb = await GetUserAsync(userId);
        if(userInDb.Email == TenancyConstants.Root.Email)
        {
            throw new ConflictException(["Not allowed to delete Admin User from Root Tenant"]);
        }
        _context.Users.Remove(userInDb);
        await _context.SaveChangesAsync();
        return userId;
    }

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken ct)
    {
        var usersInDb = await _userManager.Users.ToListAsync(ct);
        return usersInDb.Adapt<List<UserResponse>>();
    }

    public async Task<UserResponse> GetByIdAsync(string userId, CancellationToken ct)
    {
        var userInDb = await GetUserAsync(userId);
        return userInDb.Adapt<UserResponse>();
    }

    public async Task<List<string>> GetUserPermissionsAsync(string userId, CancellationToken ct)
    {
        var userInDb = await GetUserAsync(userId);
        var userRoleNames = await _userManager.GetRolesAsync(userInDb);
        var permissions = new List<string>();
        foreach(var role in await _roleManager
            .Roles
            .Where(r => userRoleNames.Contains(r.Name))
            .ToListAsync(ct))
        {
            permissions.AddRange(await _context
                .RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permission)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(ct));
        }
        return permissions.Distinct().ToList();
    }

    public async Task<List<UserRoleResponse>> GetUserRolesAsync(string userId, CancellationToken ct)
    {
        var userInDb = await GetUserAsync(userId);

        var userRoles = new List<UserRoleResponse>();

        var rolesInDb = await _roleManager.Roles.ToListAsync(ct);

        foreach(var role in rolesInDb)
        {
            userRoles.Add(new UserRoleResponse
            {
                RoleId = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsAssigned = await _userManager.IsInRoleAsync(userInDb, role.Name)
            });
        }
        return userRoles;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<bool> IsPermissionAssignedAsync(string userId, string permission, CancellationToken ct)
    {
        return (await GetUserPermissionsAsync(userId, ct)).Contains(permission);
    }

    public async Task<string> UpdateAsync(UpdateUserRequest request)
    {
        var userInDb = await GetUserAsync(request.Id);

        userInDb.FirstName = request.FirstName;
        userInDb.LastName = request.LastName;
        userInDb.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(userInDb);
        if(!result.Succeeded)
        {
            throw new IdentityException(
                IdentityHelper.GetIdentityResultErrorDescriptions(result));
        }
        return userInDb.Id;
    }

    private async Task<ApplicationUser> GetUserAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(["User does not exist"]);
    }
}
