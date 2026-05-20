namespace CarWorkshop.Tests;

public class AppointmentTests
{
    [Fact]
    public void Constructor_WhenScheduledAtIsInPast_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => TestData.Appointment(scheduledAt: DateTime.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public void Constructor_WhenProblemDescriptionIsEmpty_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => new CarWorkshop.Domain.Entities.Appointment(
                Guid.NewGuid(),
                TestData.Vehicle(),
                DateTime.UtcNow.AddDays(1),
                ""));
    }
}
