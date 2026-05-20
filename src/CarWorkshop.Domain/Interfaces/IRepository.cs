namespace CarWorkshop.Domain.Interfaces;

public interface IRepository<T>
{
    void Add(T item);
    T? GetById(Guid id);
    IEnumerable<T> GetAll();
}
