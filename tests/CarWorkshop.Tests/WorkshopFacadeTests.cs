using CarWorkshop.Application.Facade;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.ValueObjects;
using CarWorkshop.Infrastructure.Repositories;

namespace CarWorkshop.Tests;

public class WorkshopFacadeTests
{
    [Fact]
    public void GetAppointmentsForDay_ExcludesCancelledAppointments()
    {
        var setup = CreateSetup();
        var kept = TestData.Appointment(setup.Vehicle, DateTime.UtcNow.AddDays(1));
        var cancelled = TestData.Appointment(setup.Vehicle, kept.ScheduledAt.AddHours(2));
        cancelled.Cancel();
        setup.Appointments.Add(kept);
        setup.Appointments.Add(cancelled);

        var result = setup.Facade.GetAppointmentsForDay(kept.ScheduledAt).ToList();

        Assert.Single(result);
        Assert.Equal(kept.Id, result[0].Id);
    }

    [Fact]
    public void GetCompletedJobs_ReturnsOnlyCompletedJobs()
    {
        var setup = CreateSetup();
        var completed = TestData.StartedJob();
        completed.Complete(new Money(100, "UAH"), new Money(50, "UAH"));
        setup.Jobs.Add(completed);
        setup.Jobs.Add(TestData.Job());

        var result = setup.Facade.GetCompletedJobs().ToList();

        Assert.Single(result);
        Assert.Equal(JobStatus.Completed, result[0].Status);
    }

    [Fact]
    public void GetJobsByMechanicInPeriod_ReturnsJobsForSelectedMechanic()
    {
        var setup = CreateSetup();
        var job = TestData.Job(TestData.ConfirmedAppointment(setup.Vehicle, DateTime.UtcNow.AddDays(1)));
        job.AssignMechanic(setup.Mechanic);
        setup.Jobs.Add(job);

        var result = setup.Facade.GetJobsByMechanicInPeriod(
            setup.Mechanic.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(2)).ToList();

        Assert.Single(result);
        Assert.Equal(job.Id, result[0].Id);
    }

    [Fact]
    public void GetJobCountByMechanic_GroupsAssignedJobs()
    {
        var setup = CreateSetup();
        var job = TestData.Job(TestData.ConfirmedAppointment(setup.Vehicle, DateTime.UtcNow.AddDays(1)));
        job.AssignMechanic(setup.Mechanic);
        setup.Jobs.Add(job);

        var result = setup.Facade.GetJobCountByMechanic().Single();

        Assert.Equal(setup.Mechanic.FullName, result.Mechanic);
        Assert.Equal(1, result.JobCount);
    }

    private static Setup CreateSetup()
    {
        var customers = new InMemoryCustomerRepository();
        var vehicles = new InMemoryVehicleRepository();
        var appointments = new InMemoryAppointmentRepository();
        var jobs = new InMemoryJobRepository();
        var mechanics = new InMemoryMechanicRepository();
        var customer = TestData.Customer();
        var vehicle = TestData.Vehicle(customer);
        var mechanic = TestData.Mechanic();
        customers.Add(customer);
        vehicles.Add(vehicle);
        mechanics.Add(mechanic);

        return new Setup(
            new WorkshopFacade(customers, vehicles, appointments, jobs, mechanics),
            vehicle,
            mechanic,
            appointments,
            jobs);
    }

    private sealed record Setup(
        WorkshopFacade Facade,
        CarWorkshop.Domain.Entities.Vehicle Vehicle,
        CarWorkshop.Domain.Entities.Mechanic Mechanic,
        InMemoryAppointmentRepository Appointments,
        InMemoryJobRepository Jobs);
}
