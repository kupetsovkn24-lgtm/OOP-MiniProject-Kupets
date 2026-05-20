using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class ConfirmAppointmentUseCase
{
    private readonly IAppointmentRepository _appointments;

    public ConfirmAppointmentUseCase(IAppointmentRepository appointments)
    {
        _appointments = appointments;
    }

    public Appointment Execute(Guid appointmentId)
    {
        var appointment = _appointments.GetById(appointmentId)
            ?? throw new InvalidOperationException($"Appointment {appointmentId} not found.");

        appointment.Confirm();
        return appointment;
    }
}
