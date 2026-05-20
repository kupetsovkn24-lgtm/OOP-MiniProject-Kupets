using CarWorkshop.Domain.Enums;

namespace CarWorkshop.Domain.Observers;

public sealed class JobStatusChangedEventArgs : EventArgs
{
    public Guid JobId { get; }
    public JobStatus OldStatus { get; }
    public JobStatus NewStatus { get; }
    public DateTime ChangedAt { get; }

    public JobStatusChangedEventArgs(Guid jobId, JobStatus oldStatus, JobStatus newStatus, DateTime changedAt)
    {
        JobId = jobId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
    }
}
