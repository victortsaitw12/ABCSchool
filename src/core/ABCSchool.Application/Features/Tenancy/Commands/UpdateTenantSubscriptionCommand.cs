using System;
using ABCSchool.Application.Wrappers;
using MediatR;

namespace ABCSchool.Application.Features.Tenancy.Commands;

public class UpdateTenantSubscriptionCommand: IRequest<IResponseWrapper>
{
    public UpdateTenantSubscriptioRequest UpdateTenantSubscription { get; set; }
}

public class UpdateTenantSubscriptionCommandHandler: IRequestHandler<UpdateTenantSubscriptionCommand, IResponseWrapper>
{
    private readonly ITenantService _tenantService;

    public UpdateTenantSubscriptionCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public async Task<IResponseWrapper> Handle(UpdateTenantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = await _tenantService.UpdateSubscriptionAsync(request.UpdateTenantSubscription);
        return await ResponseWrapper<string>.SuccessAsync(data: tenantId, "Tenant subscription updated successfully");
    }
}
