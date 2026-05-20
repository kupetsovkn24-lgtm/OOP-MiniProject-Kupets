using CarWorkshop.Application.UseCases;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.Interfaces;
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
}
