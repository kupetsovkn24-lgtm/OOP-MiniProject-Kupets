namespace CarWorkshop.Domain.Exceptions;

public sealed class InvalidMoneyAmountException : DomainException
{
    public InvalidMoneyAmountException(decimal amount)
        : base($"Money amount cannot be negative. Got: {amount}.") { }
}
