using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Pricing;

// Labor = hours worked × hourly rate; parts are passed separately
public class HourlyPricingStrategy : IPricingStrategy
{
    private readonly Money _hourlyRate;
    private readonly Money _partsCost;

    public HourlyPricingStrategy(Money hourlyRate, Money partsCost)
    {
        _hourlyRate = hourlyRate;
        _partsCost = partsCost;
    }

    public PricingResult Calculate(Job job)
    {
        if (job.StartedAt is null)
            throw new InvalidOperationException("Cannot calculate hourly cost: job has not been started.");

        var hours = (decimal)(DateTime.UtcNow - job.StartedAt.Value).TotalHours;
        var laborCost = new Money(Math.Round(hours * _hourlyRate.Amount, 2), _hourlyRate.Currency);
        return new PricingResult(laborCost, _partsCost);
    }
}
