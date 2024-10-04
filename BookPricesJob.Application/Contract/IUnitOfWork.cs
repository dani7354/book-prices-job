namespace BookPricesJob.Application.Contract;

public interface IUnitOfWork
{
    IJobRepository JobRepository { get; }
    Task<int> Complete();
    Task Dispose();
}
