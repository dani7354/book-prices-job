using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class StatisticsService(IUnitOfWork unitOfWork) : IStatisticsService
{
    private static readonly HashSet<JobRunStatus> FinishedStatuses = [ JobRunStatus.Completed, JobRunStatus.Failed ];
    
    public async Task<IList<JobRunCountsByStatus>> GetJobRunCountsByJob()
    {
        var jobs = await unitOfWork.JobRepository.GetJobs();
        var jobRunCounts = await unitOfWork.JobRunRepository.GetJobRunCountsByJob();
        var jobRunCountsByJob = CreateJobRunCountsByJob(jobRunCounts);
        
        AddMissingJobsToJobRunCountsByJob(jobRunCountsByJob, jobs);
        AddMissingStatusesToJobRunCountsByJob(jobRunCountsByJob);
        
        return jobRunCountsByJob.Values.ToList();
    }

    private static Dictionary<string, JobRunCountsByStatus> CreateJobRunCountsByJob(
            IList<(string, string, string, int)> jobRunCounts)
    {
        var jobRunCountsByJob = new Dictionary<string, JobRunCountsByStatus>();
        foreach (var (jobId, jobName, status, count) in jobRunCounts)
        {
            if (jobRunCountsByJob.TryGetValue(jobId, out var countsByStatus))
            {
                countsByStatus.CountsByStatus[status] = count;
                countsByStatus.PercentagesByStatus[status] = (float) countsByStatus.CountsByStatus.Values.Sum() / 100 * count;
            }
            else
            {
                countsByStatus = new JobRunCountsByStatus(
                    JobId: jobId,
                    JobName: jobName,
                    CountsByStatus: new Dictionary<string, int>{ { status, count } },
                    PercentagesByStatus: new Dictionary<string, float> { {status, 100f } });

                jobRunCountsByJob[jobId] = countsByStatus;
            }
        }
        
        return jobRunCountsByJob;
    }
    
    private static void AddMissingJobsToJobRunCountsByJob(
        Dictionary<string, JobRunCountsByStatus> jobRunCountsByJob,
        IList<Job> allJobs)
    {
        foreach (var job in allJobs)
        {
            var jobId = job.Id!;
            var countByStatuses = FinishedStatuses
                .ToDictionary(s => s.ToString(), _ => 0);
            var percentagesByStatuses = FinishedStatuses
                .ToDictionary(s => s.ToString(), _ => 0.0f);
            
            jobRunCountsByJob.TryAdd(jobId, new JobRunCountsByStatus(
                JobId: jobId,
                JobName: job.Name,
                CountsByStatus: countByStatuses,
                PercentagesByStatus: percentagesByStatuses));
        }
    }
    
    private static void AddMissingStatusesToJobRunCountsByJob(Dictionary<string, JobRunCountsByStatus> jobRunCountsByJob)
    {
        foreach (var status in FinishedStatuses)
        {
            var statusString = status.ToString();
            foreach (var jobRunCountsByJobEntry in jobRunCountsByJob.Values)
            {
                jobRunCountsByJobEntry.CountsByStatus.TryAdd(statusString, 0);
                jobRunCountsByJobEntry.PercentagesByStatus.TryAdd(statusString, 0.0f);
            }
        }
    }
}