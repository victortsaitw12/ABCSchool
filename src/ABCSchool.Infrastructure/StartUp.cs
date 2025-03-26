using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using ABCSchool.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using ABCSchool.Infrastructure.Identity;

namespace ABCSchool.Infrastructure;

public static class StartUp
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddDbContext<TenantDbContext>(options =>options
                .UseSqlServer(config.GetConnectionString("DefaultConnection")))
            .AddMultiTenant<ABCSchoolTenantInfo>()
                .WithHeaderStrategy(TenancyConstants.TenantIdName)
                .WithClaimStrategy(TenancyConstants.TenantIdName)
                .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>()
                .Services
            .AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(config.GetConnectionString("DefaultConnection")))
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddIdentityService();
    }

    public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }

    internal static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        return services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .Services;
    }
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app.UseMultiTenant();
    }
}
