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
    [ProducesResponseType<AddressResponse>(StatusCodes.Status201Created)]
    public ActionResult<AddressResponse> Create(CreateAddressRequest request)
    {
        Guid addressId = Guid.NewGuid();
        Address address = service.Create(addressId, request.AddressLine1, request.AddressLine2, request.AddressLine3, request.Postcode);
        return CreatedAtAction(nameof(Get), new { addressId = address.AddressId }, address.ToResponse());
    }

    [HttpGet("{addressId}")]
    [ProducesResponseType<AddressResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AddressResponse> Get(Guid addressId)
    {
        Address address = service.Get(addressId);
        return Ok(address.ToResponse());
    }
}
