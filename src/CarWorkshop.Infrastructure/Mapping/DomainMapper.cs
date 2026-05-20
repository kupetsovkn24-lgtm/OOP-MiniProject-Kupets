using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.ValueObjects;
using CarWorkshop.Infrastructure.Dto;

namespace CarWorkshop.Infrastructure.Mapping;

public static class DomainMapper
{
    // ── Domain → DTO ──────────────────────────────────────────────────────────

    public static CustomerDto ToDto(Customer c) =>
        new(c.Id, c.FullName, c.Phone.Value);

    public static MechanicDto ToDto(Mechanic m) =>
        new(m.Id, m.FullName, m.Specialization.ToString());

    public static MoneyDto ToDto(Money m) =>
        new(m.Amount, m.Currency);

    public static VehicleDto ToDto(Vehicle v) =>
        new(v.Id, v.Vin.Value, v.Brand, v.Model, v.Owner.Id);

    public static AppointmentDto ToDto(Appointment a) =>
        new(a.Id, a.Vehicle.Id, a.ScheduledAt, a.ProblemDescription, a.Status.ToString());

    public static JobDto ToDto(Job j) =>
        new(j.Id,
            j.Appointment.Id,
            j.Mechanic?.Id,
            j.Status.ToString(),
            j.StartedAt,
            j.CompletedAt,
            j.LaborCost is not null ? ToDto(j.LaborCost) : null,
            j.PartsCost is not null ? ToDto(j.PartsCost) : null);

    // ── DTO → Domain ──────────────────────────────────────────────────────────

    public static Customer ToDomain(CustomerDto dto) =>
        new(dto.Id, dto.FullName, new PhoneNumber(dto.Phone));

    public static Mechanic ToDomain(MechanicDto dto) =>
        new(dto.Id, dto.FullName, Enum.Parse<MechanicSpecialization>(dto.Specialization));

    public static Money ToDomain(MoneyDto dto) =>
        new(dto.Amount, dto.Currency);

    public static Vehicle ToDomain(VehicleDto dto, IReadOnlyDictionary<Guid, Customer> customers) =>
        new(dto.Id, new Vin(dto.Vin), dto.Brand, dto.Model, customers[dto.OwnerId]);

    public static Appointment ToDomain(
        AppointmentDto dto, IReadOnlyDictionary<Guid, Vehicle> vehicles) =>
        Appointment.Reconstruct(
            dto.Id,
            vehicles[dto.VehicleId],
            dto.ScheduledAt,
            dto.ProblemDescription,
            Enum.Parse<AppointmentStatus>(dto.Status));

    public static Job ToDomain(
        JobDto dto,
        IReadOnlyDictionary<Guid, Appointment> appointments,
        IReadOnlyDictionary<Guid, Mechanic> mechanics) =>
        Job.Reconstruct(
            dto.Id,
            appointments[dto.AppointmentId],
            dto.MechanicId.HasValue ? mechanics[dto.MechanicId.Value] : null,
            Enum.Parse<JobStatus>(dto.Status),
            dto.StartedAt,
            dto.CompletedAt,
            dto.LaborCost is not null ? ToDomain(dto.LaborCost) : null,
            dto.PartsCost is not null ? ToDomain(dto.PartsCost) : null);
}
