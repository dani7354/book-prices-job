using BookPricesJob.Application.Contract;
using BookPricesJob.Data.DatabaseContext;

namespace BookPricesJob.Data.Repository;

public class UnitOfWork(DefaultDatabaseContext dataContext) : IUnitOfWork
{
    public IJobRepository JobRepository { get; } = new JobRepository(dataContext);

    public IJobRunRepository JobRunRepository { get; } = new JobRunRepository(dataContext);

    public Task<int> Complete()
    { 
        return dataContext.SaveChangesAsync();
    }
}
