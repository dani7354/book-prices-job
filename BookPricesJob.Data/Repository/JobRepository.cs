using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using BookPricesJob.Data.Mapper;


namespace BookPricesJob.Data.Repository;

public class JobRepository(DatabaseContext dbContext) : IJobRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<string> Add(Job job)
    {
        var jobEntity = JobMapper.MapJobToEntity(job);
        await _dbContext.AddAsync(jobEntity);

        return jobEntity.Id.ToString();
    }

    public async Task Delete(string id)
    {
        var jobEntity = await _dbContext.Job
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (jobEntity is null) return;
        _dbContext.Job.Remove(jobEntity);
    }

    public async Task<IList<Job>> GetAll()
    {
        var jobEntities = await _dbContext.Job
            .AsNoTracking()
            .Include(j => j.JobRuns)
            .ThenInclude(x => x.Arguments)
            .ToListAsync();

        return jobEntities.Select(JobMapper.MapJobToDomain).ToList();
    }

    public async Task<Job?> GetById(string id)
    {
        var jobEntity = await _dbContext.Job
            .AsNoTracking()
            .Include(j => j.JobRuns)
            .ThenInclude(x => x.Arguments)
            .FirstOrDefaultAsync(j => j.Id == id);

        return jobEntity == null ? null : JobMapper.MapJobToDomain(jobEntity);
    }

    public async Task<IList<Job>> GetJobs()
    {
        var jobs = await _dbContext.Job
            .AsNoTracking()
            .Include(j => j.JobRuns)
            .ThenInclude(x => x.Arguments)
            .ToListAsync();

        return jobs.Select(JobMapper.MapJobToDomain).ToList();
    }

    public async Task Update(Job job)
    {
        var existingEntity = await _dbContext.Job.FirstOrDefaultAsync(x => x.Id == job.Id);
        var jobEntity = JobMapper.MapJobToEntity(job, existingEntity);

        _dbContext.Update(jobEntity);
    }
}
