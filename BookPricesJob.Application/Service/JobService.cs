using System;
using System.Collections.Generic;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class JobService(IUnitOfWork unitOfWork) : IJobService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    #region Job

    public async Task<IList<Job>> GetJobs()
    {
        return await _unitOfWork.JobRepository.GetJobs();
    }

    public async Task<Job?> GetJobById(string id)
    {
        return await _unitOfWork.JobRepository.GetById(id);
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
    }

    public async Task UpdateJob(Job job)
    {
        await _unitOfWork.JobRepository.Update(job);
        await _unitOfWork.Complete();
    }
    #endregion

    #region JobRun
    public async Task<IList<JobRun>> GetJobRuns()
    {
        return await _unitOfWork.JobRunRepository.GetAll();
    }

    public async Task<JobRun?> GetJobRunById(string id)
    {
        return await _unitOfWork.JobRunRepository.GetById(id);
    }

    public async Task<string> CreateJobRun(JobRun jobRun)
    {
        var id = await _unitOfWork.JobRunRepository.Add(jobRun);
        await _unitOfWork.Complete();

        return id;
    }

    public async Task UpdateJobRun(JobRun jobRun)
    {
        await _unitOfWork.JobRunRepository.Update(jobRun);
        await _unitOfWork.Complete();
    }

    public async Task DeleteJobRun(string id)
    {
        await _unitOfWork.JobRunRepository.Delete(id);
        await _unitOfWork.Complete();
    }

    public async Task<IList<(JobRun, Job)>> FilterJobRuns(string? jobId, JobRunStatus? status, JobRunPriority? priority, int? limit)
    {
        var jobRuns = await _unitOfWork.JobRunRepository.FilterBy(jobId, status, priority, limit);

        var jobsForJobRuns = new Dictionary<string, Job>();
        foreach (var id in jobRuns.Select(x => x.JobId).Distinct())
        {
            var job = await _unitOfWork.JobRepository.GetById(id);
            if (job is not null)
                jobsForJobRuns.Add(id, job);
        }

        return jobRuns.Select(x => (x, jobsForJobRuns[x.JobId])).ToList();
    }

    #endregion
}
