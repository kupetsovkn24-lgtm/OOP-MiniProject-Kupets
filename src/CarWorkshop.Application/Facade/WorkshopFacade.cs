using CarWorkshop.Application.Extensions;
using CarWorkshop.Application.UseCases;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Interfaces;
using CarWorkshop.Domain.Observers;
using CarWorkshop.Domain.Pricing;

namespace CarWorkshop.Application.Facade;

public class WorkshopFacade
{
    private readonly IAppointmentRepository _appointments;
    private readonly IJobRepository _jobs;
    private readonly IMechanicRepository _mechanics;

    private readonly CreateAppointmentUseCase _createAppointment;
    private readonly ConfirmAppointmentUseCase _confirmAppointment;
    private readonly CreateJobFromAppointmentUseCase _createJob;
    private readonly AssignMechanicUseCase _assignMechanic;
    private readonly StartJobUseCase _startJob;
    private readonly CompleteJobUseCase _completeJob;

    private readonly List<IJobStatusObserver> _jobObservers = new();

    public WorkshopFacade(
        ICustomerRepository customers,
        IVehicleRepository vehicles,
        IAppointmentRepository appointments,
        IJobRepository jobs,
        IMechanicRepository mechanics)
    {
        _appointments = appointments;
        _jobs = jobs;
        _mechanics = mechanics;

        _createAppointment  = new CreateAppointmentUseCase(appointments, vehicles);
        _confirmAppointment = new ConfirmAppointmentUseCase(appointments);
        _createJob          = new CreateJobFromAppointmentUseCase(appointments, jobs);
        _assignMechanic     = new AssignMechanicUseCase(jobs, mechanics);
        _startJob           = new StartJobUseCase(jobs);
        _completeJob        = new CompleteJobUseCase(jobs);
    }

    // ── Observer wiring ───────────────────────────────────────────────────────

    public void AddJobObserver(IJobStatusObserver observer) =>
        _jobObservers.Add(observer);

    // ── Use case delegation ───────────────────────────────────────────────────

    public Appointment CreateAppointment(Guid vehicleId, DateTime scheduledAt, string problem) =>
        _createAppointment.Execute(vehicleId, scheduledAt, problem);

    public Appointment ConfirmAppointment(Guid appointmentId) =>
        _confirmAppointment.Execute(appointmentId);

    public Job CreateJob(Guid appointmentId)
    {
        var job = _createJob.Execute(appointmentId);
        foreach (var observer in _jobObservers)
            observer.Subscribe(job);
        return job;
    }

    public Job AssignMechanic(Guid jobId, Guid mechanicId) =>
        _assignMechanic.Execute(jobId, mechanicId);

    public Job StartJob(Guid jobId) =>
        _startJob.Execute(jobId);

    public Job CompleteJob(Guid jobId, IPricingStrategy strategy) =>
        _completeJob.Execute(jobId, strategy);

    // ── LINQ queries ──────────────────────────────────────────────────────────

    public IEnumerable<Appointment> GetAppointmentsForDay(DateTime day) =>
        _appointments.GetAll()
            .Where(a => a.ScheduledAt.Date == day.Date
                     && a.Status != AppointmentStatus.Cancelled);

    public IEnumerable<Job> GetJobsByMechanicInPeriod(Guid mechanicId, DateTime from, DateTime to) =>
        _jobs.GetAll()
            .Where(j => j.Mechanic?.Id == mechanicId
                     && j.Appointment.ScheduledAt >= from
                     && j.Appointment.ScheduledAt <= to);

    public decimal GetRevenueForPeriod(DateTime from, DateTime to) =>
        _jobs.GetAll().CompletedRevenueForPeriod(from, to);

    public IEnumerable<(string Mechanic, int JobCount)> GetJobCountByMechanic() =>
        _jobs.GetAll()
            .JoinWithMechanics(_mechanics.GetAll())
            .GroupBy(x => x.Mechanic.FullName)
            .Select(g => (Mechanic: g.Key, JobCount: g.Count()));
}
