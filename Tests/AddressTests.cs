using InsuranceDomain;
using InsuranceDomain.Exceptions;

namespace InsureApi.Tests;

public sealed class AddressTests
{
    [Test]
    public void AddressLine1_IsRequired()
    {
        Guid addressId = Guid.NewGuid();
        Assert.Throws<DomainRuleException>(() => new Address(addressId, "", null, null, "SW1A 1AA"));
    }

    [Test]
    public void Postcode_IsRequired()
    {
        Guid addressId = Guid.NewGuid();
        Assert.Throws<DomainRuleException>(() => new Address(addressId, "123 Main St", null, null, ""));
    }

    [Test]
    public void Postcode_LongerThanEightCharacters_IsRejected()
    {
        Guid addressId = Guid.NewGuid();
        Assert.Throws<DomainRuleException>(() => new Address(addressId, "123 Main St", null, null, "SW1A 1AAA"));
    }
}
