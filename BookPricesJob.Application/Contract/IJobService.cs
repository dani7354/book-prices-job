using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IJobService
{
    Task<string> CreateJob(Job job);
    Task<IList<Job>> GetJobs();
    Task<Job?> GetById(string id);
    Task DeleteJob(string id);
    Task UpdateJob(Job job);
}
