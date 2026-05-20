using CarWorkshop.Domain.Pricing;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Tests;

public class PricingStrategyTests
{
    [Fact]
    public void FixedPricingStrategy_ReturnsFixedLaborAndZeroParts()
    {
        var strategy = new FixedPricingStrategy(new Money(1500, "UAH"));

        var result = strategy.Calculate(TestData.Job());

        Assert.Equal(new Money(1500, "UAH"), result.LaborCost);
        Assert.Equal(new Money(0, "UAH"), result.PartsCost);
    }

    [Fact]
    public void PartsAndLaborPricingStrategy_ReturnsConfiguredCosts()
    {
        var strategy = new PartsAndLaborPricingStrategy(
            new Money(1200, "UAH"),
            new Money(3500, "UAH"));

        var result = strategy.Calculate(TestData.Job());

        Assert.Equal(new Money(1200, "UAH"), result.LaborCost);
        Assert.Equal(new Money(3500, "UAH"), result.PartsCost);
    }

    [Fact]
    public void HourlyPricingStrategy_WhenJobIsNotStarted_Throws()
    {
        var strategy = new HourlyPricingStrategy(new Money(500, "UAH"), new Money(200, "UAH"));

        Assert.Throws<InvalidOperationException>(() => strategy.Calculate(TestData.Job()));
    }
}
