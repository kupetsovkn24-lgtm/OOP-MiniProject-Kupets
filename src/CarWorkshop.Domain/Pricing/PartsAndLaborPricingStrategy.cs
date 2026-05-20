using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Pricing;

// Explicit labor + parts — typical for mixed repair jobs
public class PartsAndLaborPricingStrategy : IPricingStrategy
{
    private readonly Money _laborCost;
    private readonly Money _partsCost;

    public PartsAndLaborPricingStrategy(Money laborCost, Money partsCost)
    {
        _laborCost = laborCost;
        _partsCost = partsCost;
    }

    public PricingResult Calculate(Job job) => new(_laborCost, _partsCost);
}
