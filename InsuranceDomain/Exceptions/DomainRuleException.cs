namespace InsuranceDomain.Exceptions;

public sealed class DomainRuleException(string message) : Exception(message);
