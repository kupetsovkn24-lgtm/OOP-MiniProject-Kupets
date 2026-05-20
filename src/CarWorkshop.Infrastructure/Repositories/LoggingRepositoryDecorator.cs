using CarWorkshop.Domain.Interfaces;

namespace CarWorkshop.Infrastructure.Repositories;

public class LoggingRepositoryDecorator<T> : IRepository<T>
{
    private readonly IRepository<T> _inner;
    private readonly Action<string> _log;

    public LoggingRepositoryDecorator(IRepository<T> inner, Action<string> log)
    {
        _inner = inner;
        _log = log;
    }

    public void Add(T item)
    {
        _log($"[{typeof(T).Name}] Add");
        _inner.Add(item);
    }

    public T? GetById(Guid id)
    {
        _log($"[{typeof(T).Name}] GetById({id})");
        return _inner.GetById(id);
    }

    public IEnumerable<T> GetAll()
    {
        _log($"[{typeof(T).Name}] GetAll");
        return _inner.GetAll();
    }
}
