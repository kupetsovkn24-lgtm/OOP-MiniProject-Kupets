using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Domain.Pricing;

public interface IPricingStrategy
{
    PricingResult Calculate(Job job);
}
