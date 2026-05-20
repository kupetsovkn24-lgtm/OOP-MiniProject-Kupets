using CarWorkshop.Domain.Observers;

namespace CarWorkshop.Tests;

public class JobStatusHistoryObserverTests
{
    [Fact]
    public void Indexer_ReturnsHistoryEntryByIndex()
    {
        var observer = new JobStatusHistoryObserver();
        var job = TestData.AssignedJob();
        observer.Subscribe(job);

        job.Start();

        Assert.Single(observer.History);
        Assert.Equal(job.Id, observer[0].JobId);
        Assert.Equal("Assigned", observer[0].OldStatus);
        Assert.Equal("InProgress", observer[0].NewStatus);
    }
}
