using CarWorkshop.Domain.Enums;

namespace CarWorkshop.Domain.Exceptions;

public sealed class InvalidAppointmentTransitionException : DomainException
{
    public InvalidAppointmentTransitionException(AppointmentStatus current, string action)
        : base($"Cannot {action} appointment in status {current}.") { }
}
