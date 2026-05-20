using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    bool IsTimeSlotAvailable(Guid vehicleId, DateTime scheduledAt);
}
