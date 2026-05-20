namespace CarWorkshop.Domain.ValueObjects;

public sealed class Vin : IEquatable<Vin>
{
    public string Value { get; }

    public Vin(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("VIN cannot be empty.", nameof(value));

        var normalized = value.ToUpperInvariant().Trim();

        // VIN: 17 alphanumeric characters, I/O/Q are not allowed
        if (normalized.Length != 17 || !IsValidVin(normalized))
            throw new ArgumentException($"'{value}' is not a valid VIN.", nameof(value));

        Value = normalized;
    }

    private static bool IsValidVin(string vin)
    {
        foreach (var ch in vin)
        {
            if (ch == 'I' || ch == 'O' || ch == 'Q') return false;
            if (!char.IsLetterOrDigit(ch)) return false;
        }
        return true;
    }

    public bool Equals(Vin? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as Vin);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
