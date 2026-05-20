using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; }
    public Vin Vin { get; }
    public string Brand { get; }
    public string Model { get; }
    public Customer Owner { get; }

    public Vehicle(Guid id, Vin vin, string brand, string model, Customer owner)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Brand cannot be empty.", nameof(brand));
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model cannot be empty.", nameof(model));

        Id = id;
        Vin = vin;
        Brand = brand;
        Model = model;
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }
}
