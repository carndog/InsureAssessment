using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class PaymentsController(RetrievePolicyService service) : ControllerBase
{
    [HttpGet("{uniqueReference}/payments")]
    [ProducesResponseType<IEnumerable<PaymentResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<PaymentResponse>> GetPayments(string uniqueReference)
    {
        Policy policy = service.GetPolicy(uniqueReference);
        return Ok(policy.Payments.Select(p => p.ToResponse()));
    }
}
