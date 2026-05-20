namespace CarWorkshop.Domain.Exceptions;

public sealed class JobAlreadyCompletedException : DomainException
{
    public JobAlreadyCompletedException(Guid jobId)
        : base($"Job {jobId} is already completed and cannot be modified.") { }
}
