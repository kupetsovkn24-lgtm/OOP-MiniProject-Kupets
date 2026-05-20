using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Tests;

public class JobStatusTests
{
    [Theory]
    [InlineData(JobStatus.Created, JobStatus.Assigned)]
    [InlineData(JobStatus.Created, JobStatus.Cancelled)]
    [InlineData(JobStatus.Assigned, JobStatus.InProgress)]
    [InlineData(JobStatus.Assigned, JobStatus.Cancelled)]
    [InlineData(JobStatus.InProgress, JobStatus.Completed)]
    public void TransitionTo_WhenTransitionIsAllowed_ChangesStatus(JobStatus current, JobStatus next)
    {
        var job = CreateJobInStatus(current);

        job.TransitionTo(next);

        Assert.Equal(next, job.Status);
    }

    [Theory]
    [InlineData(JobStatus.Created, JobStatus.InProgress)]
    [InlineData(JobStatus.Created, JobStatus.Completed)]
    [InlineData(JobStatus.Assigned, JobStatus.Completed)]
    [InlineData(JobStatus.InProgress, JobStatus.Assigned)]
    [InlineData(JobStatus.InProgress, JobStatus.Cancelled)]
    public void TransitionTo_WhenTransitionIsNotAllowed_Throws(JobStatus current, JobStatus next)
    {
        var job = CreateJobInStatus(current);

        Assert.Throws<InvalidJobTransitionException>(() => job.TransitionTo(next));
    }

    [Fact]
    public void TransitionTo_WhenJobIsCompleted_ThrowsJobAlreadyCompleted()
    {
        var job = CreateJobInStatus(JobStatus.Completed);

        Assert.Throws<JobAlreadyCompletedException>(() => job.TransitionTo(JobStatus.Cancelled));
    }

    [Fact]
    public void Complete_WhenCostsHaveDifferentCurrencies_DoesNotCompleteJob()
    {
        var job = TestData.StartedJob();

        Assert.Throws<CurrencyMismatchException>(
            () => job.Complete(new Money(100, "UAH"), new Money(10, "USD")));

        Assert.Equal(JobStatus.InProgress, job.Status);
    }

    private static CarWorkshop.Domain.Entities.Job CreateJobInStatus(JobStatus status)
    {
        var job = TestData.Job();

        switch (status)
        {
            case JobStatus.Created:
                return job;

            case JobStatus.Assigned:
                job.TransitionTo(JobStatus.Assigned);
                return job;

            case JobStatus.InProgress:
                job.TransitionTo(JobStatus.Assigned);
                job.TransitionTo(JobStatus.InProgress);
                return job;

            case JobStatus.Completed:
                job.TransitionTo(JobStatus.Assigned);
                job.TransitionTo(JobStatus.InProgress);
                job.Complete(new Money(100, "UAH"), new Money(20, "UAH"));
                return job;

            case JobStatus.Cancelled:
                job.TransitionTo(JobStatus.Cancelled);
                return job;

            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }
}
