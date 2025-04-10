using System;
using ABCSchool.Infrastructure.Tenancy;

namespace ABCSchool.Infrastructure.OpenApi;

public class TenantHeaderAttribute: SwaggerHeaderAttribute
{
    public TenantHeaderAttribute()
        : base(headerName:TenancyConstants.TenantIdName, 
                description:"Enter your tenant name to access this API", 
                defaultValue: string.Empty, 
                isRequired:true)
    {
    }
}
