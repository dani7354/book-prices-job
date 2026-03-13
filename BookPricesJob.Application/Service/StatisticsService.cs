using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class StatisticsService(IUnitOfWork unitOfWork) : IStatisticsService
{
    private static readonly HashSet<JobRunStatus> FinishedStatuses = [ JobRunStatus.Completed, JobRunStatus.Failed ];
    
    public async Task<IList<JobRunCountsByStatus>> GetJobRunCountsByJob(int days)
    {
        var jobs = await unitOfWork.JobRepository.GetJobs();
        if (!jobs.Any())
            return new List<JobRunCountsByStatus>();
        
        var afterDate = DateTime.Now.AddDays(-days);
        var jobRunCounts = await unitOfWork.JobRunRepository.GetJobRunCountsByJob(
            FinishedStatuses,
            afterDate);
        
        var jobRunCountsByJob = CreateJobRunCountsByJob(jobRunCounts);
        
        AddMissingJobsToJobRunCountsByJob(jobRunCountsByJob, jobs);
        
        return jobRunCountsByJob.Values.ToList();
    }

    private static Dictionary<string, JobRunCountsByStatus> CreateJobRunCountsByJob(
        Dictionary<string, List<(string, string, string, int)>> jobRunCounts)
    {
        var jobRunCountsByJob = new Dictionary<string, JobRunCountsByStatus>();
        foreach (var (jobId, countsForJob) in jobRunCounts)
        {
            if (!jobRunCountsByJob.TryGetValue(jobId, out var countsByStatus))
            {
                var (_, jobName, _, _) = countsForJob.First();
                jobRunCountsByJob[jobId] = countsByStatus = JobRunCountsByStatus.CreateEmpty(jobId, jobName);
            }

            var totalJobRunCount = countsForJob.Sum(z => z.Item4);
            foreach (var (_, _, status, count) in countsForJob)
            {
                countsByStatus.CountsByStatus[status] = count;
                countsByStatus.PercentagesByStatus[status] = (float) count / totalJobRunCount * 100;
            }

            if (countsByStatus.CountsByStatus.Count == FinishedStatuses.Count)
                continue;
            
            AddZeroCountsForMissingStatuses(countsByStatus);
        }
        
        return jobRunCountsByJob;
    }
    
    private static void AddMissingJobsToJobRunCountsByJob(
        Dictionary<string, JobRunCountsByStatus> jobRunCountsByJob,
        IEnumerable<Job> allJobs)
    {
        foreach (var job in allJobs)
        {
            var jobId = job.Id!;
            if (jobRunCountsByJob.ContainsKey(jobId))
                continue;

            var jobRunCountByStatus = JobRunCountsByStatus.CreateEmpty(jobId, job.Name);
            
            AddZeroCountsForMissingStatuses(jobRunCountByStatus);
            jobRunCountsByJob[jobId] = jobRunCountByStatus;
        }
    }
    
    private static void AddZeroCountsForMissingStatuses(JobRunCountsByStatus jobRunCountsByStatus)
    {
        foreach (var status in FinishedStatuses)
        {
            var statusString = status.ToString();
            jobRunCountsByStatus.CountsByStatus.TryAdd(statusString, 0);
            jobRunCountsByStatus.PercentagesByStatus.TryAdd(statusString, 0.0f);
        }
    }
}