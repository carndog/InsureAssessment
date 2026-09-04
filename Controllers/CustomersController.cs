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
    public ActionResult<CustomerResponse> Create(CreateCustomerRequest request)
    {
        Guid customerId = Guid.NewGuid();
        Customer customer = service.Create(customerId, request.FirstName, request.LastName, request.DateOfBirth);
        return CreatedAtAction(nameof(Get), new { customerId = customer.CustomerId }, customer.ToResponse());
    }

    [HttpGet("{customerId}")]
    public ActionResult<CustomerResponse> Get(Guid customerId)
    {
        Customer customer = service.Get(customerId);
        return Ok(customer.ToResponse());
    }

    [HttpGet("{customerId}/policies")]
    public ActionResult<IEnumerable<PolicyResponse>> GetPolicies(Guid customerId)
    {
        Policy[] policies = service.GetPolicies(customerId);
        return Ok(policies.Select(p => p.ToResponse()));
    }
}
