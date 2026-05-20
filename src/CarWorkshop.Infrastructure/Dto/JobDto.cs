namespace CarWorkshop.Infrastructure.Dto;

public record JobDto(
    Guid Id,
    Guid AppointmentId,
    Guid? MechanicId,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    MoneyDto? LaborCost,
    MoneyDto? PartsCost);
