using BookPricesJob.Application.Contract;
using BookPricesJob.Application.DatabaseContext;

namespace BookPricesJob.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContextBase _dataContext;

    public IJobRepository JobRepository { get; }

    public IJobRunRepository JobRunRepository { get; }

    public UnitOfWork(DatabaseContextBase dataContext)
    {
        _dataContext = dataContext;
        JobRunRepository = new JobRunRepository(dataContext);
        JobRepository = new JobRepository(dataContext);
    }

    public async Task<int> Complete()
    {
        return await _dataContext.SaveChangesAsync();
    }

    public async Task Dispose()
    {
        await _dataContext.DisposeAsync();
    }
}
