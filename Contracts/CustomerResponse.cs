namespace InsureApi.Contracts;

public sealed record CustomerResponse(
    string CustomerId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);
