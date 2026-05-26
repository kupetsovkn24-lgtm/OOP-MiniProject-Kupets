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
    public void FilterCompletedJobs_AppliesConditionAndExcludesIncompleteJobs()
    {
        var expensiveJob = CompletedJob(1200, 3500);
        var inexpensiveJob = CompletedJob(1000, 500);
        var incompleteJob = TestData.AssignedJob();

        var result = new[] { expensiveJob, inexpensiveJob, incompleteJob }
            .FilterCompletedJobs(job => job.TotalCost().Amount >= 4000)
            .ToList();

        Assert.Single(result);
        Assert.Equal(expensiveJob.Id, result[0].Id);
    }

    [Theory]
    [InlineData(0, 2, 6200)]
    [InlineData(4000, 1, 4700)]
    [InlineData(5000, 0, 0)]
    public void BuildCompletedJobsReport_FiltersAndAggregatesRevenue(
        decimal minimumTotal,
        int expectedCount,
        decimal expectedRevenue)
    {
        var jobs = new[]
        {
            CompletedJob(1200, 3500),
            CompletedJob(1000, 500),
            TestData.AssignedJob()
        };

        var result = jobs.BuildCompletedJobsReport(job => job.TotalCost().Amount >= minimumTotal);

        Assert.Equal(expectedCount, result.JobCount);
        Assert.Equal(expectedRevenue, result.TotalRevenue);
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

    private static CarWorkshop.Domain.Entities.Job CompletedJob(decimal labor, decimal parts)
    {
        var job = TestData.StartedJob();
        job.Complete(new Money(labor, "UAH"), new Money(parts, "UAH"));
        return job;
    }
}
