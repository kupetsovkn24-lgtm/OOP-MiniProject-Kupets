using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Exceptions;
using CarWorkshop.Domain.Observers;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Entities;

public class Job
{
    // Allowed transitions: key = current status, value = set of valid next statuses
    private static readonly Dictionary<JobStatus, HashSet<JobStatus>> _allowedTransitions = new()
    {
        [JobStatus.Created]    = [JobStatus.Assigned, JobStatus.Cancelled],
        [JobStatus.Assigned]   = [JobStatus.InProgress, JobStatus.Cancelled],
        [JobStatus.InProgress] = [JobStatus.Completed],
        [JobStatus.Completed]  = [],
        [JobStatus.Cancelled]  = [],
    };

    public event EventHandler<JobStatusChangedEventArgs>? StatusChanged;

    public Guid Id { get; }
    public Appointment Appointment { get; }
    public Mechanic? Mechanic { get; private set; }
    public JobStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Money? LaborCost { get; private set; }
    public Money? PartsCost { get; private set; }

    internal Job(Guid id, Appointment appointment)
    {
        Id = id;
        Appointment = appointment ?? throw new ArgumentNullException(nameof(appointment));
        Status = JobStatus.Created;
    }

    // Used only by mapper when reconstructing from persistence — bypasses state machine
    internal static Job Reconstruct(
        Guid id, Appointment appointment, Mechanic? mechanic,
        JobStatus status, DateTime? startedAt, DateTime? completedAt,
        Money? laborCost, Money? partsCost)
    {
        var job = new Job(id, appointment);
        job.Status = status;
        job.Mechanic = mechanic;
        job.StartedAt = startedAt;
        job.CompletedAt = completedAt;
        job.LaborCost = laborCost;
        job.PartsCost = partsCost;
        return job;
    }

    public Money TotalCost()
    {
        if (LaborCost is null || PartsCost is null)
            throw new InvalidOperationException("Job does not have a calculated cost yet.");

        return LaborCost + PartsCost;
    }

    public void AssignMechanic(Mechanic mechanic)
    {
        ArgumentNullException.ThrowIfNull(mechanic);
        TransitionTo(JobStatus.Assigned);
        Mechanic = mechanic;
    }

    public void Start()
    {
        TransitionTo(JobStatus.InProgress);
        StartedAt = DateTime.UtcNow;
    }

    public void Complete(Money laborCost, Money partsCost)
    {
        if (laborCost is null) throw new ArgumentNullException(nameof(laborCost));
        if (partsCost is null) throw new ArgumentNullException(nameof(partsCost));

        _ = laborCost + partsCost;

        TransitionTo(JobStatus.Completed);
        LaborCost = laborCost;
        PartsCost = partsCost;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        TransitionTo(JobStatus.Cancelled);
    }

    public void TransitionTo(JobStatus newStatus)
    {
        if (Status == JobStatus.Completed)
            throw new JobAlreadyCompletedException(Id);

        if (!_allowedTransitions[Status].Contains(newStatus))
            throw new InvalidJobTransitionException(Status, newStatus);

        var oldStatus = Status;
        Status = newStatus;
        StatusChanged?.Invoke(this, new JobStatusChangedEventArgs(Id, oldStatus, newStatus, DateTime.UtcNow));
    }
}
