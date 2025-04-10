using System;

namespace ABCSchool.Infrastructure.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerHeaderAttribute: Attribute
{
    public string HeaderName { get; set; }
    public string Description { get; set; }
    public string DefaultValue { get; set; }
    public bool IsRequired { get; set; }

    public SwaggerHeaderAttribute(string headerName, string description, string defaultValue, bool isRequired)
    {
        HeaderName = headerName;
        Description = description;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
    }
}
