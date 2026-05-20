namespace CarWorkshop.Domain.Entities;

public abstract class Person
{
    public Guid Id { get; }
    public string FullName { get; }

    protected Person(Guid id, string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));

        Id = id;
        FullName = fullName.Trim();
    }

    public virtual string GetDisplayName() => FullName;

    public override string ToString() => GetDisplayName();
}
