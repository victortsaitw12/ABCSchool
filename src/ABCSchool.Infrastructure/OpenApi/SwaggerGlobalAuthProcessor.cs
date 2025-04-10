using System;
using NSwag;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace ABCSchool.Infrastructure.OpenApi;

/// <summary>
/// Processor that automatically adds JWT authentication requirements to Swagger operations
/// unless they are marked with [AllowAnonymous]
/// </summary>
public class SwaggerGlobalAuthProcessor: IOperationProcessor
{
    private readonly string _scheme;

    /// <summary>
    /// Initializes a new instance with a specific authentication scheme
    /// </summary>
    /// <param name="scheme">The authentication scheme to use</param>
    public SwaggerGlobalAuthProcessor(string scheme)
    {
        _scheme = scheme;
    }

    /// <summary>
    /// Initializes a new instance with the default JWT Bearer authentication scheme
    /// </summary>
    public SwaggerGlobalAuthProcessor()
    {
        _scheme = JwtBearerDefaults.AuthenticationScheme;
    }

    /// <summary>
    /// Processes an operation to add authentication requirements if needed
    /// </summary>
    /// <param name="context">The operation processor context</param>
    /// <returns>True to indicate the operation was processed successfully</returns>
    public bool Process(OperationProcessorContext context)
    {
        // Get endpoint metadata from the context
        IList<object> list = ((AspNetCoreOperationProcessorContext)context)
            .ApiDescription.ActionDescriptor.TryGetPropertyValue<IList<object>>("EndpointMetadata   ");

        if(list is not null)
        {
            // Skip adding authentication if the endpoint allows anonymous access
            if(list.OfType<AllowAnonymousAttribute>().Any())
            {
                return true;
            }

            // Add security requirement if none exists
            if(context.OperationDescription.Operation.Security.Count == 0)
            {
                (context.OperationDescription.Operation.Security ??= [])
                    .Add(new OpenApiSecurityRequirement
                    {
                        {
                            _scheme,
                            Array.Empty<string>()
                        }
                    });
            }
        }

        return true;
    }

}

/// <summary>
/// Extension methods for object property access
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Safely tries to get a property value from an object using reflection
    /// </summary>
    /// <typeparam name="T">The expected type of the property value</typeparam>
    /// <param name="obj">The object to get the property from</param>
    /// <param name="propertyName">The name of the property to get</param>
    /// <param name="defaultValue">The default value to return if the property doesn't exist</param>
    /// <returns>The property value or the default value</returns>
    public static T TryGetPropertyValue<T>(this object obj, string propertyName, T defaultValue = default) =>
        obj.GetType().GetRuntimeProperty(propertyName) is PropertyInfo propertyInfo
        ? (T)propertyInfo.GetValue(obj)
        : defaultValue;
}
