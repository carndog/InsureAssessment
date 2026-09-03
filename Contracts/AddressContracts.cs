namespace InsureApi.Contracts;

public sealed record CreateAddressRequest(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string Postcode);

public sealed record AddressResponse(
    string AddressId,
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string Postcode);
