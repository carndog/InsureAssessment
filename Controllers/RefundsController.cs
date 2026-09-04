using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class RefundsController(RetrievePolicyService service) : ControllerBase
{
    [HttpGet("{uniqueReference}/refunds")]
    public ActionResult<IEnumerable<RefundResponse>> GetRefunds(string uniqueReference)
    {
        Policy policy = service.GetPolicy(uniqueReference);
        return Ok(policy.Refunds.Select(r => r.ToResponse()));
    }
}
