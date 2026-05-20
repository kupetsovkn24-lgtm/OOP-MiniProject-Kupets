using CarWorkshop.Domain.Enums;

namespace CarWorkshop.Domain.Exceptions;

public sealed class InvalidJobTransitionException : DomainException
{
    public InvalidJobTransitionException(JobStatus from, JobStatus to)
        : base($"Cannot transition Job from {from} to {to}.") { }
}
