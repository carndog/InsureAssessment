using InsuranceDomain.Services;
using InsureApi.Contracts;
using InsureApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using InsuranceDomain;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/addresses")]
public sealed class AddressesController(AddressService service) : ControllerBase
{
    [HttpPost]
    public ActionResult<AddressResponse> Create(CreateAddressRequest request)
    {
        Guid addressId = Guid.NewGuid();
        Address address = service.Create(addressId, request.AddressLine1, request.AddressLine2, request.AddressLine3, request.Postcode);
        return CreatedAtAction(nameof(Get), new { addressId = address.AddressId }, address.ToResponse());
    }

    [HttpGet("{addressId}")]
    public ActionResult<AddressResponse> Get(Guid addressId)
    {
        Address address = service.Get(addressId);
        return Ok(address.ToResponse());
    }
}
