using CarWorkshop.Domain.Enums;

namespace CarWorkshop.Domain.Entities;

public class Mechanic : Person
{
    public MechanicSpecialization Specialization { get; }

    public Mechanic(Guid id, string fullName, MechanicSpecialization specialization)
        : base(id, fullName)
    {
        Specialization = specialization;
    }

    public override string GetDisplayName() =>
        $"{base.GetDisplayName()} [{Specialization}]";
}
