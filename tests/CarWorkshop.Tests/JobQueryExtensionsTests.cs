using CarWorkshop.Application.Extensions;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Tests;

public class JobQueryExtensionsTests
{
    [Fact]
    public void CompletedRevenueForPeriod_UsesAggregateAndIgnoresIncompleteJobs()
    {
        var completedJob = TestData.StartedJob();
        completedJob.Complete(new Money(1200, "UAH"), new Money(3500, "UAH"));

        var incompleteJob = TestData.AssignedJob();
        var from = DateTime.UtcNow.AddMinutes(-1);
        var to = DateTime.UtcNow.AddMinutes(1);

        var revenue = new[] { completedJob, incompleteJob }
            .CompletedRevenueForPeriod(from, to);

        Assert.Equal(4700, revenue);
    }

    [Fact]
    public void JoinWithMechanics_ReturnsJobsMatchedToMechanicEntities()
    {
        var mechanic = TestData.Mechanic();
        var assignedJob = TestData.Job();
        assignedJob.AssignMechanic(mechanic);

        var unassignedJob = TestData.Job();

        var result = new[] { assignedJob, unassignedJob }
            .JoinWithMechanics(new[] { mechanic })
            .Single();

        Assert.Equal(mechanic.Id, result.Mechanic.Id);
        Assert.Equal(assignedJob.Id, result.Job.Id);
    }
}
