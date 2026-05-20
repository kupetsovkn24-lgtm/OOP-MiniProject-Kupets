using CarWorkshop.Domain.Entities;

namespace CarWorkshop.Domain.Observers;

public interface IJobStatusObserver
{
    void Subscribe(Job job);
}
