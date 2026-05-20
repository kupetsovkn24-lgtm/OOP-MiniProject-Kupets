using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Pricing;

// Flat fee for the whole job — labor absorbs the full amount, parts = 0
public class FixedPricingStrategy : IPricingStrategy
{
    private readonly Money _fixedPrice;

    public FixedPricingStrategy(Money fixedPrice)
    {
        _fixedPrice = fixedPrice;
    }

    public PricingResult Calculate(Job job) =>
        new(_fixedPrice, new Money(0, _fixedPrice.Currency));
}
