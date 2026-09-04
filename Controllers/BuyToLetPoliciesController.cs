using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/policies/buytolet")]
public sealed class BuyToLetPoliciesController(SellPolicyService service) : ControllerBase
{
    [HttpPost]
    public ActionResult<PolicyResponse> Sell(SellBuyToLetPolicyRequest request)
    {
        SellPolicyDetails details = new SellPolicyDetails(
            request.StartDate,
            request.EndDate,
            request.Amount,
            request.AutoRenew,
            request.CustomerIds.ToArray(),
            request.AddressId,
            request.Payment.PaymentReference,
            request.Payment.Type,
            request.Payment.Amount);

        Policy policy = service.SellBuyToLet(details);

        return CreatedAtAction(
            actionName: nameof(PoliciesController.Get),
            controllerName: "Policies",
            routeValues: new { uniqueReference = policy.UniqueReference },
            value: policy.ToResponse());
    }
}
