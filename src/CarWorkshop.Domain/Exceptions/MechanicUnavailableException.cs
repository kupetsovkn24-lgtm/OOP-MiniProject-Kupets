namespace CarWorkshop.Domain.Exceptions;

public sealed class MechanicUnavailableException : DomainException
{
    public MechanicUnavailableException(Guid mechanicId)
        : base($"Mechanic {mechanicId} is not available for the requested time slot.") { }
}
