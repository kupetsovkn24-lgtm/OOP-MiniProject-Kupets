namespace CarWorkshop.Domain.Exceptions;

public sealed class CurrencyMismatchException : DomainException
{
    public CurrencyMismatchException(string left, string right)
        : base($"Cannot operate on Money with different currencies: {left} and {right}.") { }
}
