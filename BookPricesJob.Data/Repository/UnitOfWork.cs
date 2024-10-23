using BookPricesJob.Application.Contract;

namespace BookPricesJob.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _dataContext;

    public IJobRepository JobRepository { get; }

    public IJobRunRepository JobRunRepository { get; }

    public UnitOfWork(DatabaseContext dataContext)
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
