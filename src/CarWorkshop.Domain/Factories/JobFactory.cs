using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Exceptions;

namespace CarWorkshop.Domain.Factories;

public static class JobFactory
{
    public static Job CreateFromAppointment(Appointment appointment)
    {
        if (appointment is null)
            throw new ArgumentNullException(nameof(appointment));

        if (appointment.Status != AppointmentStatus.Confirmed)
            throw new InvalidAppointmentTransitionException(
                appointment.Status, "create a job from");

        return new Job(Guid.NewGuid(), appointment);
    }
}
