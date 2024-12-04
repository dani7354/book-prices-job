using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class JobService(IUnitOfWork unitOfWork, ICache cache) : IJobService
{
    private readonly TimeSpan _jobCacheExpiry = TimeSpan.FromHours(3);
    private readonly TimeSpan _jobRunCacheExpiry = TimeSpan.FromMinutes(30);

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICache _cache = cache;

    #region Job

    public async Task<IList<Job>> GetJobs()
    {
        return await _unitOfWork.JobRepository.GetJobs();
    }

    public async Task<Job?> GetJobById(string id)
    {
        var jobCacheKey = CacheKeyGenerator.GenerateJobKey(id);
        var job = await _cache.GetAsync<Job>(jobCacheKey);
        if (job is not null)
            return job;

        job = await _unitOfWork.JobRepository.GetById(id);
        if (job is not null)
            await _cache.SetAsync(jobCacheKey, job, _jobCacheExpiry);

        return job;
    }

    public async Task<string> CreateJob(Job job)
    {
        var id = await _unitOfWork.JobRepository.Add(job);
        await _unitOfWork.Complete();

        return id;
    }

    public async Task DeleteJob(string id)
    {
        await _unitOfWork.JobRepository.Delete(id);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(id));
    }

    public async Task UpdateJob(Job job)
    {
        await _unitOfWork.JobRepository.Update(job);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(job.Id!));
    }
    #endregion

    #region JobRun
    public async Task<IList<JobRun>> GetJobRuns()
    {
        return await _unitOfWork.JobRunRepository.GetAll();
    }

    public async Task<JobRun?> GetJobRunById(string id)
    {
        var jobRunCacheKey = CacheKeyGenerator.GenerateJobRunKey(id);
        var jobRun = await _cache.GetAsync<JobRun>(jobRunCacheKey);
        if (jobRun is not null)
            return jobRun;

        jobRun = await _unitOfWork.JobRunRepository.GetById(id);
        if (jobRun is not null)
            await _cache.SetAsync(jobRunCacheKey, jobRun, _jobRunCacheExpiry);

        return jobRun;
    }

    public async Task<string> CreateJobRun(JobRun jobRun)
    {
        var id = await _unitOfWork.JobRunRepository.Add(jobRun);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(jobRun.JobId));

        return id;
    }

    public async Task UpdateJobRun(JobRun jobRun)
    {
        await _unitOfWork.JobRunRepository.Update(jobRun);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunKey(jobRun.Id!));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(jobRun.JobId));
    }

    public async Task DeleteJobRun(string id)
    {
        await _unitOfWork.JobRunRepository.Delete(id);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunKey(id));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(id));
    }

    public async Task<IList<(JobRun, Job)>> FilterJobRuns(
        string? jobId,
        JobRunStatus? status,
        JobRunPriority? priority,
        int? limit)
    {
        var jobRuns = await _unitOfWork.JobRunRepository.FilterBy(jobId, status, priority, limit);

        var jobsForJobRuns = new Dictionary<string, Job>();
        foreach (var id in jobRuns.Select(x => x.JobId).Distinct())
        {
            var job = await _unitOfWork.JobRepository.GetById(id);
            if (job is not null)
                jobsForJobRuns.Add(id, job);
        }

        return jobRuns
            .Where(x => jobsForJobRuns.ContainsKey(x.JobId))
            .Select(x => (x, jobsForJobRuns[x.JobId]))
            .ToList();
    }

    #endregion
}
