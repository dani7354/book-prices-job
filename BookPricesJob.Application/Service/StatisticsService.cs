using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class StatisticsService(IUnitOfWork unitOfWork) : IStatisticsService
{
    public async Task<IList<JobRunCountsByStatus>> GetJobRunCountsByJob()
    {
        var jobRunCounts = await unitOfWork.JobRunRepository.GetJobRunCountsByJob();
        
        var (jobRunCountsByJob, uniqueStatuses) = CreateJobRunCountsByJob(
            jobRunCounts);
        
        AddMissingStatusesToJobRunCountsByJob(jobRunCountsByJob, uniqueStatuses);
        
        return jobRunCountsByJob.Values.ToList();
    }

    private static (
        Dictionary<string, JobRunCountsByStatus>, HashSet<JobRunStatus>) CreateJobRunCountsByJob(
            List<(string, string, string, int)> jobRunCounts)
    {
        var uniqueStatuses = new HashSet<JobRunStatus>();
        var jobRunCountsByJob = new Dictionary<string, JobRunCountsByStatus>();
        foreach (var (jobId, jobName, status, count) in jobRunCounts)
        {
            uniqueStatuses.Add(Enum.Parse<JobRunStatus>(status));
            if (jobRunCountsByJob.TryGetValue(jobId, out var countsByStatus))
            {
                countsByStatus.CountsByStatus[status] = count;
            }
            else
            {
                jobRunCountsByJob[jobId] = new JobRunCountsByStatus(
                    jobId,
                    jobName,
                    new Dictionary<string, int>{ { status, count } });
            }
        }
        
        return (jobRunCountsByJob, uniqueStatuses);
    }
    
    private static void AddMissingStatusesToJobRunCountsByJob(
        Dictionary<string, JobRunCountsByStatus> jobRunCountsByJob,
        HashSet<JobRunStatus> uniqueStatuses)
    {
        foreach (var status in uniqueStatuses)
        {
            var statusString = status.ToString();
            foreach (var jobRunCountsByJobEntry in jobRunCountsByJob.Values)
                jobRunCountsByJobEntry.CountsByStatus.TryAdd(statusString, 0);
        }
    }
}