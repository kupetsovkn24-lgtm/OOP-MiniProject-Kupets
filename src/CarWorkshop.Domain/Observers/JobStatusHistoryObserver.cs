using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Domain.Observers;

public sealed class JobStatusHistoryObserver : IJobStatusObserver
{
    public sealed record HistoryEntry(
        Guid JobId,
        string OldStatus,
        string NewStatus,
        DateTime ChangedAt);

    private readonly List<HistoryEntry> _history = new();

    public IReadOnlyList<HistoryEntry> History => _history.AsReadOnly();

    public HistoryEntry this[int index] => _history[index];

    public void Subscribe(Job job) =>
        job.StatusChanged += (_, args) => _history.Add(
            new HistoryEntry(args.JobId, args.OldStatus.ToString(), args.NewStatus.ToString(), args.ChangedAt));
}
