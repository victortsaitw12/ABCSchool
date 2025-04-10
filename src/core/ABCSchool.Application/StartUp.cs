using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using ABCSchool.Application.Pipelines;
namespace ABCSchool.Application;

public static class StartUp
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return services
            .AddValidatorsFromAssembly(assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>))
            .AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
            });
    }
}
