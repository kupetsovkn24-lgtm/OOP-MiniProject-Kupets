using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T>
{
    private readonly List<T> _items = new();
    private readonly Func<T, Guid> _idSelector;

    public InMemoryRepository(Func<T, Guid> idSelector)
    {
        _idSelector = idSelector;
    }

    public void Add(T item) => _items.Add(item);

    public T? GetById(Guid id) =>
        _items.FirstOrDefault(x => _idSelector(x) == id);

    public IEnumerable<T> GetAll() => _items.AsReadOnly();

    protected List<T> Items => _items;
}
