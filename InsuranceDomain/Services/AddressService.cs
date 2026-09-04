using InsuranceDomain.DataLayer;

namespace InsuranceDomain.Services;

public sealed class AddressService(InsuranceStore store)
{
    public Address Create(
        Guid addressId,
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string postcode)
    {
        Address address = new Address(addressId, addressLine1, addressLine2, addressLine3, postcode);
        store.Add(address);
        return address;
    }

    public Address Get(Guid addressId) => store.GetAddress(addressId);
}
