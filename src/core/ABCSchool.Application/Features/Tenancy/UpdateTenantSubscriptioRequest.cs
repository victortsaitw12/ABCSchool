using System;

namespace ABCSchool.Application.Features.Tenancy;

public class UpdateTenantSubscriptioRequest
{
    public string TenantId { get; set; }
    public DateTime NewExpiryDate { get; set; }
}
