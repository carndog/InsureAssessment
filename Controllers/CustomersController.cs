using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(CustomerService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status201Created)]
    public ActionResult<CustomerResponse> Create(CreateCustomerRequest request)
    {
        Guid customerId = Guid.NewGuid();
        Customer customer = service.Create(customerId, request.FirstName, request.LastName, request.DateOfBirth);
        return CreatedAtAction(nameof(Get), new { customerId = customer.CustomerId }, customer.ToResponse());
    }

    [HttpGet("{customerId}")]
    [ProducesResponseType<CustomerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CustomerResponse> Get(Guid customerId)
    {
        Customer customer = service.Get(customerId);
        return Ok(customer.ToResponse());
    }

    [HttpGet("{customerId}/policies")]
    [ProducesResponseType<IEnumerable<PolicyResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<PolicyResponse>> GetPolicies(Guid customerId, bool includeLapsed = true)
    {
        Customer customer = service.Get(customerId);
        IReadOnlyCollection<Policy> policies = service.GetPolicies(customerId);
        return Ok(policies.Select(p => p.ToResponse()));
    }
}
