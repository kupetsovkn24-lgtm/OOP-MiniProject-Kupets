using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class CancelAppointmentUseCase
{
    private readonly IAppointmentRepository _appointments;
    private readonly IJobRepository _jobs;

    public CancelAppointmentUseCase(IAppointmentRepository appointments, IJobRepository jobs)
    {
        _appointments = appointments;
        _jobs = jobs;
    }

    public Appointment Execute(Guid appointmentId)
    {
        var appointment = _appointments.GetById(appointmentId)
            ?? throw new InvalidOperationException($"Appointment {appointmentId} not found.");

        if (_jobs.GetAll().Any(job => job.Appointment.Id == appointmentId))
            throw new InvalidOperationException("Cannot cancel an appointment after a job was created.");

        appointment.Cancel();
        return appointment;
    }
}
