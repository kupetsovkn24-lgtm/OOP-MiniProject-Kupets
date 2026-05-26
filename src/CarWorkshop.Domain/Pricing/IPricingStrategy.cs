using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Domain.Pricing;

/// <summary>
/// Calculates labor and parts costs without changing the job lifecycle.
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Calculates costs for a job according to the selected pricing rule.
    /// </summary>
    PricingResult Calculate(Job job);
}
