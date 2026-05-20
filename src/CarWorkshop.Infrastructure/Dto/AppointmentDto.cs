namespace CarWorkshop.Infrastructure.Dto;

public record AppointmentDto(
    Guid Id,
    Guid VehicleId,
    DateTime ScheduledAt,
    string ProblemDescription,
    string Status);
