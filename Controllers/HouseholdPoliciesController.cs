using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/policies/households")]
public sealed class HouseholdPoliciesController(SellPolicyService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PolicyResponse>(StatusCodes.Status201Created)]
    public ActionResult<PolicyResponse> Sell(SellHouseholdPolicyRequest request)
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

        Policy policy = service.SellHousehold(details);

        return CreatedAtAction(
            actionName: nameof(PoliciesController.Get),
            controllerName: "Policies",
            routeValues: new { uniqueReference = policy.UniqueReference },
            value: policy.ToResponse());
    }
}
