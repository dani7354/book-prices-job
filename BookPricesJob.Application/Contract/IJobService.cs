using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IJobService
{
    Task<string> CreateJob(Job job);
    Task<IList<Job>> GetJobs();
    Task<Job?> GetJobById(string id);
    Task DeleteJob(string id);
    Task UpdateJob(Job job);

    Task<IList<JobRun>> GetJobRuns();
    Task<JobRun?> GetJobRunById(string id);
}
