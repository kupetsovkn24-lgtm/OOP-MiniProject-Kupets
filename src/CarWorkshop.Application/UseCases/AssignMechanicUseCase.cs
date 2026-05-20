using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class AssignMechanicUseCase
{
    private readonly IJobRepository _jobs;
    private readonly IMechanicRepository _mechanics;

    public AssignMechanicUseCase(IJobRepository jobs, IMechanicRepository mechanics)
    {
        _jobs = jobs;
        _mechanics = mechanics;
    }

    public Job Execute(Guid jobId, Guid mechanicId)
    {
        var job = _jobs.GetById(jobId)
            ?? throw new InvalidOperationException($"Job {jobId} not found.");

        var mechanic = _mechanics.GetById(mechanicId)
            ?? throw new InvalidOperationException($"Mechanic {mechanicId} not found.");

        if (!_jobs.IsMechanicAvailable(mechanicId, job.Appointment.ScheduledAt))
            throw new MechanicUnavailableException(mechanicId);

        job.AssignMechanic(mechanic);
        return job;
    }
}
