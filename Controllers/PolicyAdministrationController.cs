using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class PolicyAdministrationController(
    CalculateCancellationCostService calculateCancellationCostService,
    CancelPolicyService cancelPolicyService,
    RenewPolicyService renewPolicyService) : ControllerBase
{
    [HttpPost("{uniqueReference}/cancellation-quotes")]
    public ActionResult<CancellationQuoteResponse> CalculateCancellation(string uniqueReference, CalculateCancellationRequest request)
    {
        CancellationQuote quote = calculateCancellationCostService.Execute(uniqueReference, request.CancellationDate);
        return Ok(quote.ToResponse());
    }

    [HttpPut("{uniqueReference}/cancellation")]
    public ActionResult<RefundResponse> Cancel(string uniqueReference, CancelPolicyRequest request)
    {
        Refund refund = cancelPolicyService.Execute(uniqueReference, request.CancellationDate);
        return Ok(refund.ToResponse());
    }

    [HttpPost("{uniqueReference}/renewals")]
    public ActionResult<PolicyResponse> Renew(string uniqueReference, RenewPolicyRequest request)
    {
        RenewalResult result = renewPolicyService.Execute(uniqueReference, request.StartDate, request.EndDate, request.Amount);
        return CreatedAtAction(
            actionName: nameof(PoliciesController.Get),
            controllerName: "Policies",
            routeValues: new { uniqueReference = result.Policy.UniqueReference },
            value: result.Policy.ToResponse());
    }
}
