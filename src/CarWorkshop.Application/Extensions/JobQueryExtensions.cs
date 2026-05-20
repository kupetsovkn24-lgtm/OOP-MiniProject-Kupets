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
