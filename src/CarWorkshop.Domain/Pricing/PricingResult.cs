using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Pricing;

public record PricingResult(Money LaborCost, Money PartsCost);
