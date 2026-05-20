using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Entities;

public class Customer : Person
{
    public PhoneNumber Phone { get; }

    public Customer(Guid id, string fullName, PhoneNumber phone)
        : base(id, fullName)
    {
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
    }

    public override string GetDisplayName() =>
        $"{base.GetDisplayName()} ({Phone})";
}
