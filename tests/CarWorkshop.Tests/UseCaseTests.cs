using CarWorkshop.Application.UseCases;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.Interfaces;
using CarWorkshop.Domain.ValueObjects;
using Moq;

namespace CarWorkshop.Tests;

public class UseCaseTests
{
    [Fact]
    public void CreateAppointment_WhenTimeSlotIsBusy_ThrowsAndDoesNotAdd()
    {
        var vehicle = TestData.Vehicle();
        var scheduledAt = DateTime.UtcNow.AddDays(1);

        var appointments = new Mock<IAppointmentRepository>();
        appointments
            .Setup(r => r.IsTimeSlotAvailable(vehicle.Id, scheduledAt))
            .Returns(false);

        var vehicles = new Mock<IVehicleRepository>();
        vehicles
            .Setup(r => r.GetById(vehicle.Id))
            .Returns(vehicle);

        var useCase = new CreateAppointmentUseCase(appointments.Object, vehicles.Object);

        Assert.Throws<TimeSlotConflictException>(
            () => useCase.Execute(vehicle.Id, scheduledAt, "Engine noise"));

        appointments.Verify(r => r.Add(It.IsAny<Appointment>()), Times.Never);
    }

    [Fact]
    public void AssignMechanic_WhenMechanicIsBusy_ThrowsAndKeepsJobCreated()
    {
        var mechanic = TestData.Mechanic();
        var job = TestData.Job();

        var jobs = new Mock<IJobRepository>();
        jobs.Setup(r => r.GetById(job.Id)).Returns(job);
        jobs
            .Setup(r => r.IsMechanicAvailable(mechanic.Id, job.Appointment.ScheduledAt))
            .Returns(false);

        var mechanics = new Mock<IMechanicRepository>();
        mechanics.Setup(r => r.GetById(mechanic.Id)).Returns(mechanic);

        var useCase = new AssignMechanicUseCase(jobs.Object, mechanics.Object);

        Assert.Throws<MechanicUnavailableException>(() => useCase.Execute(job.Id, mechanic.Id));

        Assert.Null(job.Mechanic);
    }

    [Fact]
    public void CreateJobFromAppointment_WhenJobAlreadyExists_Throws()
    {
        var appointment = TestData.ConfirmedAppointment();
        var existingJob = TestData.Job(appointment);

        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(r => r.GetById(appointment.Id)).Returns(appointment);

        var jobs = new Mock<IJobRepository>();
        jobs.Setup(r => r.GetAll()).Returns(new[] { existingJob });

        var useCase = new CreateJobFromAppointmentUseCase(appointments.Object, jobs.Object);

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(appointment.Id));

        jobs.Verify(r => r.Add(It.IsAny<Job>()), Times.Never);
    }

    [Fact]
    public void CancelAppointment_WhenNoJobExists_SetsCancelledStatus()
    {
        var appointment = TestData.Appointment();
        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(r => r.GetById(appointment.Id)).Returns(appointment);
        var jobs = new Mock<IJobRepository>();
        jobs.Setup(r => r.GetAll()).Returns([]);

        var useCase = new CancelAppointmentUseCase(appointments.Object, jobs.Object);

        var result = useCase.Execute(appointment.Id);

        Assert.Equal(AppointmentStatus.Cancelled, result.Status);
    }

    [Fact]
    public void CancelAppointment_WhenJobExists_ThrowsAndKeepsAppointmentConfirmed()
    {
        var appointment = TestData.ConfirmedAppointment();
        var job = TestData.Job(appointment);
        var appointments = new Mock<IAppointmentRepository>();
        appointments.Setup(r => r.GetById(appointment.Id)).Returns(appointment);
        var jobs = new Mock<IJobRepository>();
        jobs.Setup(r => r.GetAll()).Returns([job]);

        var useCase = new CancelAppointmentUseCase(appointments.Object, jobs.Object);

        Assert.Throws<InvalidOperationException>(() => useCase.Execute(appointment.Id));
        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void CancelJob_WhenJobIsCreated_SetsCancelledStatus()
    {
        var job = TestData.Job();
        var jobs = new Mock<IJobRepository>();
        jobs.Setup(r => r.GetById(job.Id)).Returns(job);

        var useCase = new CancelJobUseCase(jobs.Object);

        var result = useCase.Execute(job.Id);

        Assert.Equal(JobStatus.Cancelled, result.Status);
    }

    [Fact]
    public void CancelJob_WhenJobIsCompleted_Throws()
    {
        var job = TestData.StartedJob();
        job.Complete(new Money(100, "UAH"), new Money(20, "UAH"));
        var jobs = new Mock<IJobRepository>();
        jobs.Setup(r => r.GetById(job.Id)).Returns(job);

        var useCase = new CancelJobUseCase(jobs.Object);

        Assert.Throws<JobAlreadyCompletedException>(() => useCase.Execute(job.Id));
    }
}
