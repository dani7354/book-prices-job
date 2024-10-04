using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IJobService
{
    Task<IList<Job>> GetJobs();
}
