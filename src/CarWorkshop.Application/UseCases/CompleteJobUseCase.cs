using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;
using CarWorkshop.Domain.Pricing;

namespace CarWorkshop.Application.UseCases;

public class CompleteJobUseCase
{
    private readonly IJobRepository _jobs;

    public CompleteJobUseCase(IJobRepository jobs)
    {
        _jobs = jobs;
    }

    public Job Execute(Guid jobId, IPricingStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        var job = _jobs.GetById(jobId)
            ?? throw new InvalidOperationException($"Job {jobId} not found.");

        var pricing = strategy.Calculate(job);
        job.Complete(pricing.LaborCost, pricing.PartsCost);
        return job;
    }
}
