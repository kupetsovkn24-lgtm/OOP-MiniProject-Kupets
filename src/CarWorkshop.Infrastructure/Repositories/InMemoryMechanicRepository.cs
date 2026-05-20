using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class InMemoryMechanicRepository : InMemoryRepository<Mechanic>, IMechanicRepository
{
    public InMemoryMechanicRepository() : base(m => m.Id) { }
}
