using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class CreateAppointmentUseCase
{
    private readonly IAppointmentRepository _appointments;
    private readonly IVehicleRepository _vehicles;

    public CreateAppointmentUseCase(IAppointmentRepository appointments, IVehicleRepository vehicles)
    {
        _appointments = appointments;
        _vehicles = vehicles;
    }

    public Appointment Execute(Guid vehicleId, DateTime scheduledAt, string problemDescription)
    {
        if (scheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled time must be in the future.", nameof(scheduledAt));

        var vehicle = _vehicles.GetById(vehicleId)
            ?? throw new InvalidOperationException($"Vehicle {vehicleId} not found.");

        if (!_appointments.IsTimeSlotAvailable(vehicleId, scheduledAt))
            throw new TimeSlotConflictException(scheduledAt);

        var appointment = new Appointment(Guid.NewGuid(), vehicle, scheduledAt, problemDescription);
        _appointments.Add(appointment);
        return appointment;
    }
}
