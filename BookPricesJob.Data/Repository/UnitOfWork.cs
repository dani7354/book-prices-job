using BookPricesJob.Application.Contract;

namespace BookPricesJob.Data.Repository;

public class UnitOfWork(DatabaseContext dataContext) : IUnitOfWork
{
    private readonly DatabaseContext _dataContext = dataContext;

    public IJobRepository JobRepository { get; } = new JobRepository(dataContext);

    public async Task<int> Complete()
    {
        return await _dataContext.SaveChangesAsync();
    }

    public async Task Dispose()
    {
        await _dataContext.DisposeAsync();
    }
}
