using CarWorkshop.Domain.Entities;
using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class InMemoryCustomerRepository : InMemoryRepository<Customer>, ICustomerRepository
{
    public InMemoryCustomerRepository() : base(c => c.Id) { }
}
