using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Application.Reports;

/// <summary>
/// Summarizes completed jobs selected by an analytical filter.
/// </summary>
public sealed record CompletedJobsReport(IReadOnlyList<Job> Jobs, decimal TotalRevenue)
{
    /// <summary>
    /// Gets the number of completed jobs included in the report.
    /// </summary>
    public int JobCount => Jobs.Count;
}
