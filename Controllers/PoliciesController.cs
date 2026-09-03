using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class PoliciesController(RetrievePolicyService service) : ControllerBase
{
    [HttpGet("{uniqueReference}")]
    [ProducesResponseType<PolicyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PolicyResponse> Get(string uniqueReference)
    {
        Policy policy = service.GetPolicy(uniqueReference);
        return Ok(policy.ToResponse());
    }
}
