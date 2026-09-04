using InsuranceDomain.Exceptions;

namespace InsuranceDomain;

public sealed class Address
{
    public Address(
        Guid addressId,
        string addressLine1,
        string? addressLine2,
        string? addressLine3,
        string postcode)
    {
        if (addressId == Guid.Empty)
            throw new DomainRuleException("AddressId is required.");
        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new DomainRuleException("AddressLine1 is required.");
        if (string.IsNullOrWhiteSpace(postcode))
            throw new DomainRuleException("Postcode is required.");
        if (postcode.Length > 8)
            throw new DomainRuleException("Postcode cannot exceed 8 characters.");

        AddressId = addressId;
        AddressLine1 = addressLine1.Trim();
        AddressLine2 = NullIfWhiteSpace(addressLine2);
        AddressLine3 = NullIfWhiteSpace(addressLine3);
        Postcode = postcode.Trim();
    }

    public Guid AddressId { get; }
    public string AddressLine1 { get; }
    public string? AddressLine2 { get; }
    public string? AddressLine3 { get; }
    public string Postcode { get; }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
