using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Exceptions;

namespace CarWorkshop.Domain.Entities;

public class Appointment
{
    public Guid Id { get; }
    public Vehicle Vehicle { get; }
    public DateTime ScheduledAt { get; }
    public string ProblemDescription { get; }
    public AppointmentStatus Status { get; private set; }

    public Appointment(Guid id, Vehicle vehicle, DateTime scheduledAt, string problemDescription)
        : this(id, vehicle, scheduledAt, problemDescription, AppointmentStatus.Created, validateScheduledAt: true)
    {
    }

    private Appointment(
        Guid id,
        Vehicle vehicle,
        DateTime scheduledAt,
        string problemDescription,
        AppointmentStatus status,
        bool validateScheduledAt)
    {
        if (string.IsNullOrWhiteSpace(problemDescription))
            throw new ArgumentException("Problem description cannot be empty.", nameof(problemDescription));

        if (validateScheduledAt && scheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Scheduled time must be in the future.", nameof(scheduledAt));

        Id = id;
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        ScheduledAt = scheduledAt;
        ProblemDescription = problemDescription;
        Status = status;
    }

    // Used only by mapper when reconstructing from persistence — skips creation-time rules
    internal static Appointment Reconstruct(
        Guid id, Vehicle vehicle, DateTime scheduledAt,
        string problemDescription, AppointmentStatus status)
    {
        return new Appointment(
            id,
            vehicle,
            scheduledAt,
            problemDescription,
            status,
            validateScheduledAt: false);
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Created)
            throw new InvalidAppointmentTransitionException(Status, "confirm");

        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new InvalidAppointmentTransitionException(Status, "cancel");

        Status = AppointmentStatus.Cancelled;
    }
}
