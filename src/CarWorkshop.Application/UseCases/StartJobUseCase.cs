using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class StartJobUseCase
{
    private readonly IJobRepository _jobs;

    public StartJobUseCase(IJobRepository jobs)
    {
        _jobs = jobs;
    }

    public Job Execute(Guid jobId)
    {
        var job = _jobs.GetById(jobId)
            ?? throw new InvalidOperationException($"Job {jobId} not found.");

        job.Start();
        return job;
    }
}
