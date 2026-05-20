using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.Factories;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Tests;

internal static class TestData
{
    public static Customer Customer() =>
        new(Guid.NewGuid(), "Ivan Petrenko", new PhoneNumber("+380671234567"));

    public static Vehicle Vehicle(Customer? owner = null) =>
        new(Guid.NewGuid(), new Vin("1HGCM82633A123456"), "Toyota", "Camry", owner ?? Customer());

    public static Mechanic Mechanic() =>
        new(Guid.NewGuid(), "Sasha Koval", MechanicSpecialization.Engine);

    public static Appointment Appointment(Vehicle? vehicle = null, DateTime? scheduledAt = null)
    {
        return new Appointment(
            Guid.NewGuid(),
            vehicle ?? Vehicle(),
            scheduledAt ?? DateTime.UtcNow.AddDays(1),
            "Engine noise");
    }

    public static Appointment ConfirmedAppointment(Vehicle? vehicle = null, DateTime? scheduledAt = null)
    {
        var appointment = Appointment(vehicle, scheduledAt);
        appointment.Confirm();
        return appointment;
    }

    public static Job Job(Appointment? appointment = null) =>
        JobFactory.CreateFromAppointment(appointment ?? ConfirmedAppointment());

    public static Job AssignedJob()
    {
        var job = Job();
        job.AssignMechanic(Mechanic());
        return job;
    }

    public static Job StartedJob()
    {
        var job = AssignedJob();
        job.Start();
        return job;
    }
}
