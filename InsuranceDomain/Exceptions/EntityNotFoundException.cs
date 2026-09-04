namespace InsuranceDomain.Exceptions;

public sealed class EntityNotFoundException(string message) : Exception(message);
