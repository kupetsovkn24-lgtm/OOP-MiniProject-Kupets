using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class InMemoryAppointmentRepository : InMemoryRepository<Appointment>, IAppointmentRepository
{
    private static readonly TimeSpan SlotDuration = TimeSpan.FromHours(1);

    public InMemoryAppointmentRepository() : base(a => a.Id) { }

    public bool IsTimeSlotAvailable(Guid vehicleId, DateTime scheduledAt)
    {
        return !Items.Any(a =>
            a.Vehicle.Id == vehicleId &&
            a.Status != AppointmentStatus.Cancelled &&
            Math.Abs((a.ScheduledAt - scheduledAt).TotalHours) < SlotDuration.TotalHours);
    }
}
