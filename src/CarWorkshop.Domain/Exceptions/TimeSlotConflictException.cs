namespace CarWorkshop.Domain.Exceptions;

public sealed class TimeSlotConflictException : DomainException
{
    public TimeSlotConflictException(DateTime scheduledAt)
        : base($"The time slot at {scheduledAt:g} is already taken.") { }
}
