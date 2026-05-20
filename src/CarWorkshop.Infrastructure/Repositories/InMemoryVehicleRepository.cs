using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class InMemoryVehicleRepository : InMemoryRepository<Vehicle>, IVehicleRepository
{
    public InMemoryVehicleRepository() : base(v => v.Id) { }
}
