using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IStatisticsService
{
    Task<IList<JobRunCountsByStatus>> GetJobRunCountsByJob();
}