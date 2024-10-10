using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IJobService
{
    Task<int> CreateJob(Job job);
    Task<IList<Job>> GetJobs();
    Task<Job?> GetById(int id);
    Task DeleteJob(int id);
    Task UpdateJob(Job job);
}
