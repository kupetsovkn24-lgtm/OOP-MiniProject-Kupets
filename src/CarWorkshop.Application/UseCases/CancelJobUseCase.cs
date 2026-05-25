using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class CancelJobUseCase
{
    private readonly IJobRepository _jobs;

    public CancelJobUseCase(IJobRepository jobs)
    {
        _jobs = jobs;
    }

    public Job Execute(Guid jobId)
    {
        var job = _jobs.GetById(jobId)
            ?? throw new InvalidOperationException($"Job {jobId} not found.");

        job.Cancel();
        return job;
    }
}
