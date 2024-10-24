using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Contract;

public interface IJobService
{
    // Job
    Task<string> CreateJob(Job job);
    Task<IList<Job>> GetJobs();
    Task<Job?> GetJobById(string id);
    Task DeleteJob(string id);
    Task UpdateJob(Job job);

    // JobRun
    Task<IList<JobRun>> GetJobRuns();
    Task<JobRun?> GetJobRunById(string id);
    Task<string> CreateJobRun(JobRun jobRun);
    Task UpdateJobRun(JobRun jobRun);
    Task DeleteJobRun(string id);
}
