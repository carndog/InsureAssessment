using InsuranceDomain.Exceptions;

namespace InsuranceDomain;

public sealed class Customer
{
    public Customer(Guid customerId, string firstName, string lastName, DateOnly dateOfBirth)
    {
        if (customerId == Guid.Empty)
            throw new DomainRuleException("CustomerId is required.");
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainRuleException("FirstName is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainRuleException("LastName is required.");

        CustomerId = customerId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
    }

    public Guid CustomerId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateOnly DateOfBirth { get; }

    public bool IsOver16On(DateOnly date) => DateOfBirth.AddYears(16) < date;
}
