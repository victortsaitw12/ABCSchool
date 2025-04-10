using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ABCSchool.Infrastructure.Tenancy;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Builder;
using ABCSchool.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using ABCSchool.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using ABCSchool.Infrastructure.Identity.Auth;
using ABCSchool.Application.Features.Identity.Tokens;
using ABCSchool.Infrastructure.Identity.Tokens;
using ABCSchool.Application;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Net;
using ABCSchool.Application.Wrappers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ABCSchool.Infrastructure.Constants;
using System.Reflection;
using ABCSchool.Infrastructure.OpenApi;
using NSwag.Generation.Processors.Security;
using ABCSchool.Application.Features.Tenancy;
using ABCSchool.Application.Features.Schools;
using ABCSchool.Infrastructure.Schools;

namespace ABCSchool.Infrastructure;

/// <summary>
/// Infrastructure startup configuration class that sets up dependency injection and middleware.
/// This class configures multi-tenancy, database contexts, and identity services.
/// </summary>
public static class StartUp
{
    /// <summary>
    /// Configures and adds all infrastructure services to the dependency injection container.
    /// This includes database contexts, multi-tenancy, and identity services.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="config">The configuration containing connection strings and other settings</param>
    /// <returns>The configured service collection</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration config)
    {
        return services
            // Configure the tenant database context for storing tenant information
            .AddDbContext<TenantDbContext>(options =>options
                .UseSqlServer(config.GetConnectionString("DefaultConnection")))
            // Configure multi-tenancy with ABCSchoolTenantInfo
            .AddMultiTenant<ABCSchoolTenantInfo>()
                // Use header-based tenant resolution
                .WithHeaderStrategy(TenancyConstants.TenantIdName)
                // Use claim-based tenant resolution
                .WithClaimStrategy(TenancyConstants.TenantIdName)
                // Use Entity Framework Core for tenant store
                .WithEFCoreStore<TenantDbContext, ABCSchoolTenantInfo>()
                .Services
            // Configure the application database context for tenant-specific data
            .AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(config.GetConnectionString("DefaultConnection")))
            // Register database seeders as transient services
            .AddTransient<ITenantDbSeeder, TenantDbSeeder>()
            .AddTransient<ApplicationDbSeeder>()
            .AddTransient<ITenantService, TenantService>()
            .AddTransient<ISchoolService, SchoolService>()
            // Add identity services
            .AddIdentityService()
            .AddPermissions()
            .AddOpenApiDocumentation(config);
    }

    /// <summary>
    /// Initializes the database by seeding tenant and application data.
    /// This method should be called during application startup.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task AddDatabaseInitializerAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        // Initialize the database using the tenant database seeder
        await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
            .InitializeDatabaseAsync(cancellationToken);
    }

    /// <summary>
    /// Configures and adds identity services to the service collection.
    /// Sets up password requirements and user validation rules.
    /// </summary>
    /// <param name="services">The service collection to add identity services to</param>
    /// <returns>The configured service collection</returns>
    internal static IServiceCollection AddIdentityService(this IServiceCollection services)
    {
        return services
            // Configure Identity with custom user and role types
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Configure password requirements
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                // Require unique email addresses
                options.User.RequireUniqueEmail = true;
            })
            // Use Entity Framework Core for identity storage
            .AddEntityFrameworkStores<ApplicationDbContext>()
            // Add default token providers for password reset, email confirmation, etc.
            .AddDefaultTokenProviders()
            .Services
            .AddScoped<ITokenService, TokenService>();
    }

    internal static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuthorizationPolicyProvider,PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler,PermissionAuthorizationHandler>();
    }


    public static JwtSettings GetJwtSettings(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettingsConfig = config.GetSection(nameof(JwtSettings));
        services.Configure<JwtSettings>(jwtSettingsConfig);

        return jwtSettingsConfig.Get<JwtSettings>();
    }


    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);

        services
            .AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token has expired"));
                            return context.Response.WriteAsync(result);
                        }
                        else
                        {
                            if(!context.Response.HasStarted )
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An unhandled error has occurred"));
                                return context.Response.WriteAsync(result);
                            }
                            return Task.CompletedTask;
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if(!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized."));
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not authorized to access this resource."));
                        return context.Response.WriteAsync(result);
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            foreach(var prop in typeof(SchoolPermissions).GetNestedTypes()
                .SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if(propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy
                        .RequireClaim(ClaimConstants.Permission, propertyValue.ToString()));
                }
            }
        });

        return services;
    }


    internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config)
    {
        var swaggerSettings = config.GetSection(nameof(SwaggerSettings)).Get<SwaggerSettings>();

        services.AddEndpointsApiExplorer();
        _ = services.AddOpenApiDocument((document, ServiceProvider) => 
        {
            document.PostProcess = doc => 
            {
                doc.Info.Title = swaggerSettings.Title;
                doc.Info.Description = swaggerSettings.Description;
                doc.Info.Contact = new NSwag.OpenApiContact
                {
                    Name = swaggerSettings.ContactName,
                    Email = swaggerSettings.ContactEmail,
                    Url = swaggerSettings.ContactUrl,
                };
                doc.Info.License = new NSwag.OpenApiLicense
                {
                    Name = swaggerSettings.LicenseName,
                    Url = swaggerSettings.LicenseUrl,
                };
            };

            document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new NSwag.OpenApiSecurityScheme
            {
                Name="Authorization",
                Description = "Enter your Bearer token to attach it as a header on your requests",
                In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                Type= NSwag.OpenApiSecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
            });

            document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
            document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());
            document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());
        });

        return services;
    }

    /// <summary>
    /// Configures the application to use multi-tenancy middleware.
    /// This should be called in the application's startup pipeline.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The configured application builder</returns>
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app
            .UseAuthentication()
            .UseMultiTenant()
            .UseAuthorization()
            .UseOpenApiDocumentation();
    }

    internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi(options =>
        {
            options.DefaultModelExpandDepth = -1;
            options.DocExpansion = "none";
            options.TagsSorter = "alpha";
        });
        return app;
    }
}
