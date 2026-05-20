using System.Text.RegularExpressions;

namespace CarWorkshop.Domain.ValueObjects;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    // Accepts formats: +380XXXXXXXXX, 0XXXXXXXXX (10 digits), optional spaces/dashes
    private static readonly Regex _pattern =
        new(@"^\+?[\d\s\-]{7,15}$", RegexOptions.Compiled);

    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.", nameof(value));

        if (!_pattern.IsMatch(value.Trim()))
            throw new ArgumentException($"'{value}' is not a valid phone number.", nameof(value));

        Value = value.Trim();
    }

    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
