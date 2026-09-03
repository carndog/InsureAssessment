namespace InsureApi.Contracts;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);

public sealed record CustomerResponse(
    string CustomerId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);
