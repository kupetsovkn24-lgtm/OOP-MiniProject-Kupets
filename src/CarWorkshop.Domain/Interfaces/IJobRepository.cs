using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Domain.Interfaces;

public interface IJobRepository : IRepository<Job>
{
    bool IsMechanicAvailable(Guid mechanicId, DateTime scheduledAt);
    IEnumerable<Job> GetJobsByMechanic(Guid mechanicId);
}
