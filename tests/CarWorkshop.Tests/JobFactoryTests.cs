using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.Factories;

namespace CarWorkshop.Tests;

public class JobFactoryTests
{
    [Fact]
    public void CreateFromAppointment_WhenAppointmentIsNotConfirmed_Throws()
    {
        var appointment = TestData.Appointment();

        Assert.Throws<InvalidAppointmentTransitionException>(
            () => JobFactory.CreateFromAppointment(appointment));
    }

    [Fact]
    public void CreateFromAppointment_WhenAppointmentIsConfirmed_CreatesJobInCreatedStatus()
    {
        var appointment = TestData.ConfirmedAppointment();

        var job = JobFactory.CreateFromAppointment(appointment);

        Assert.Equal(appointment, job.Appointment);
        Assert.Equal(JobStatus.Created, job.Status);
        Assert.Null(job.Mechanic);
    }
}
