using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Tests;

public class MoneyTests
{
    [Fact]
    public void Constructor_WhenAmountIsNegative_Throws()
    {
        Assert.Throws<InvalidMoneyAmountException>(() => new Money(-1, "UAH"));
    }

    [Theory]
    [InlineData(100, 25, 125)]
    [InlineData(0, 500, 500)]
    [InlineData(10, 2, 12)]
    public void Add_WhenCurrenciesMatch_ReturnsSum(decimal left, decimal right, decimal expected)
    {
        var result = new Money(left, "UAH") + new Money(right, "UAH");

        Assert.Equal(new Money(expected, "UAH"), result);
    }

    [Fact]
    public void Add_WhenCurrenciesDiffer_Throws()
    {
        var uah = new Money(100, "UAH");
        var usd = new Money(10, "USD");

        Assert.Throws<CurrencyMismatchException>(() => uah + usd);
    }
}
