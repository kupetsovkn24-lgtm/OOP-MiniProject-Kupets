using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Enums;
using CarWorkshop.Domain.ValueObjects;

namespace CarWorkshop.Tests;

public class PersonInheritanceTests
{
    [Fact]
    public void Customer_InheritsPersonAndOverridesDisplayName()
    {
        Person customer = new Customer(
            Guid.NewGuid(),
            " Ivan Petrenko ",
            new PhoneNumber("+380671234567"));

        Assert.Equal("Ivan Petrenko", customer.FullName);
        Assert.Equal("Ivan Petrenko (+380671234567)", customer.GetDisplayName());
    }

    [Fact]
    public void Mechanic_InheritsPersonAndOverridesDisplayName()
    {
        Person mechanic = new Mechanic(
            Guid.NewGuid(),
            "Sasha Koval",
            MechanicSpecialization.Engine);

        Assert.Equal("Sasha Koval [Engine]", mechanic.GetDisplayName());
        Assert.Equal(mechanic.GetDisplayName(), mechanic.ToString());
    }
}
