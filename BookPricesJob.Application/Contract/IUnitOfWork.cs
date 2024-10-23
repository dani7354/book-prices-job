namespace BookPricesJob.Application.Contract;

public interface IUnitOfWork
{
    IJobRepository JobRepository { get; }
    IJobRunRepository JobRunRepository { get; }
    Task<int> Complete();
    Task Dispose();
}
