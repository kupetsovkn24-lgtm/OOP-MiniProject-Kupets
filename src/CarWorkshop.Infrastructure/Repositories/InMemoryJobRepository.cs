using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class InMemoryJobRepository : InMemoryRepository<Job>, IJobRepository
{
    private static readonly TimeSpan SlotDuration = TimeSpan.FromHours(1);

    public InMemoryJobRepository() : base(j => j.Id) { }

    public bool IsMechanicAvailable(Guid mechanicId, DateTime scheduledAt)
    {
        return !Items.Any(j =>
            j.Mechanic?.Id == mechanicId &&
            (j.Status == JobStatus.Assigned || j.Status == JobStatus.InProgress) &&
            Math.Abs((j.Appointment.ScheduledAt - scheduledAt).TotalHours) < SlotDuration.TotalHours);
    }

    public IEnumerable<Job> GetJobsByMechanic(Guid mechanicId) =>
        Items.Where(j => j.Mechanic?.Id == mechanicId);
}
