using CarWorkshop.Application.Reports;
using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;

namespace CarWorkshop.Application.Extensions;

public static class JobQueryExtensions
{
    public static decimal CompletedRevenueForPeriod(
        this IEnumerable<Job> jobs,
        DateTime from,
        DateTime to)
    {
        return jobs
            .Where(j => j.Status == JobStatus.Completed
                     && j.CompletedAt >= from
                     && j.CompletedAt <= to)
            .Select(j => j.TotalCost().Amount)
            .Aggregate(0m, (total, amount) => total + amount);
    }

    /// <summary>
    /// Selects completed jobs that satisfy an additional report condition.
    /// </summary>
    public static IEnumerable<Job> FilterCompletedJobs(
        this IEnumerable<Job> jobs,
        Func<Job, bool> condition)
    {
        ArgumentNullException.ThrowIfNull(jobs);
        ArgumentNullException.ThrowIfNull(condition);

        return jobs
            .Where(job => job.Status == JobStatus.Completed)
            .Where(condition);
    }

    /// <summary>
    /// Builds a summary for completed jobs selected by a flexible condition.
    /// </summary>
    public static CompletedJobsReport BuildCompletedJobsReport(
        this IEnumerable<Job> jobs,
        Func<Job, bool> condition)
    {
        var matchingJobs = jobs
            .FilterCompletedJobs(condition)
            .OrderByDescending(job => job.TotalCost().Amount)
            .ToList();

        return new CompletedJobsReport(
            matchingJobs,
            matchingJobs.Sum(job => job.TotalCost().Amount));
    }

    public static IEnumerable<(Mechanic Mechanic, Job Job)> JoinWithMechanics(
        this IEnumerable<Job> jobs,
        IEnumerable<Mechanic> mechanics)
    {
        return jobs
            .Where(j => j.Mechanic is not null)
            .Join(
                mechanics,
                job => job.Mechanic!.Id,
                mechanic => mechanic.Id,
                (job, mechanic) => (mechanic, job));
    }
}
