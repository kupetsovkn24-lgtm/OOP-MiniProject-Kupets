using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Factories;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Application.UseCases;

public class CreateJobFromAppointmentUseCase
{
    private readonly IAppointmentRepository _appointments;
    private readonly IJobRepository _jobs;

    public CreateJobFromAppointmentUseCase(IAppointmentRepository appointments, IJobRepository jobs)
    {
        _appointments = appointments;
        _jobs = jobs;
    }

    public Job Execute(Guid appointmentId)
    {
        var appointment = _appointments.GetById(appointmentId)
            ?? throw new InvalidOperationException($"Appointment {appointmentId} not found.");

        if (_jobs.GetAll().Any(j => j.Appointment.Id == appointmentId))
            throw new InvalidOperationException($"Job for appointment {appointmentId} already exists.");

        var job = JobFactory.CreateFromAppointment(appointment);
        _jobs.Add(job);
        return job;
    }
}
