namespace InsureApi.Contracts;

public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth);
