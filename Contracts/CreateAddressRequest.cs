namespace InsureApi.Contracts;

public sealed record CreateAddressRequest(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string Postcode);
