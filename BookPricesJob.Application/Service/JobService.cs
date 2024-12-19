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
        var jobIdListCacheKey = CacheKeyGenerator.GenerateJobListKey();
        var jobIds = await _cache.GetAsync<IList<string>>(jobIdListCacheKey);
        if (jobIds is not null)
            return await GetJobsByIds(jobIds);

        var jobsFromRepository = await _unitOfWork.JobRepository.GetJobs();
        if (jobsFromRepository.Count > 0)
            await SetJobsInCache(jobsFromRepository);

        return jobsFromRepository;
    }

    private async Task<IList<Job>> GetJobsByIds(IList<string> jobIds)
    {
        var jobs = new List<Job>();
        foreach (var id in jobIds)
        {
            var job = await GetJobById(id);
            if (job is not null)
                jobs.Add(job);
        }

        return jobs;
    }

    private async Task SetJobsInCache(IList<Job> jobs)
    {
        var jobIds = jobs.Select(x => x.Id!).ToList();
        await _cache.SetAsync(CacheKeyGenerator.GenerateJobListKey(), jobIds, _jobCacheExpiry);

        foreach (var job in jobs)
        {
            await _cache.SetAsync(
                key: CacheKeyGenerator.GenerateJobKey(job.Id!),
                value: job,
                expiry: _jobCacheExpiry);
        }
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
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobListKey());

        return id;
    }

    public async Task DeleteJob(string id)
    {
        await _unitOfWork.JobRepository.Delete(id);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(id));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobListKey());
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunListKey());
    }

    public async Task UpdateJob(Job job)
    {
        await _unitOfWork.JobRepository.Update(job);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(job.Id!));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobListKey());
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunListKey());
    }

    #endregion

    #region JobRun
    public async Task<IList<JobRun>> GetJobRuns()
    {
        var jobRunIds = await _cache.GetAsync<IList<string>>(CacheKeyGenerator.GenerateJobRunListKey());
        if (jobRunIds is not null)
            return await GetJobRunsByIds(jobRunIds);

        var jobRunsFromRepository = await _unitOfWork.JobRunRepository.GetAll();
        if (jobRunsFromRepository.Count > 0)
            await SetJobRunsInCache(jobRunsFromRepository);

        return jobRunsFromRepository;
    }

    private async Task<IList<JobRun>> GetJobRunsByIds(IList<string> jobRunIds)
    {
        var jobRuns = new List<JobRun>();
        foreach (var id in jobRunIds)
        {
            var jobRun = await GetJobRunById(id);
            if (jobRun is not null)
                jobRuns.Add(jobRun);
        }

        return jobRuns;
    }

    private async Task SetJobRunsInCache(IList<JobRun> jobRuns)
    {
        var jobRunIds = jobRuns.Select(x => x.Id!).ToList();
        await _cache.SetAsync(CacheKeyGenerator.GenerateJobListKey(), jobRunIds, _jobCacheExpiry);

        foreach (var jobRun in jobRuns)
        {
            await _cache.SetAsync(
                key: CacheKeyGenerator.GenerateJobKey(jobRun.Id!),
                value: jobRun,
                expiry: _jobCacheExpiry);
        }
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
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunListKey());
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobListKey());

        return id;
    }

    public async Task UpdateJobRun(JobRun jobRun)
    {
        await _unitOfWork.JobRunRepository.Update(jobRun);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunKey(jobRun.Id!));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(jobRun.JobId));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunListKey());
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobListKey());
    }

    public async Task DeleteJobRun(string id)
    {
        await _unitOfWork.JobRunRepository.Delete(id);
        await _unitOfWork.Complete();
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunKey(id));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobKey(id));
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobRunListKey());
        await _cache.RemoveAsync(CacheKeyGenerator.GenerateJobListKey());
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
            var job = await GetJobById(id);
            if (job is not null)
                jobsForJobRuns.Add(id, job);
        }

        return jobRuns
            .Where(x => jobsForJobRuns.ContainsKey(x.JobId))
            .Select(x => (x, jobsForJobRuns[x.JobId]))
            .OrderByDescending(x => x.x.Updated)
            .ToList();
    }

    #endregion
}
