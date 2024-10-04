using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using BookPricesJob.Data;
using BookPricesJob.Data.Mapper;
using System.Linq.Expressions;


namespace BookPricesJob.Data.Repository;

public class JobRepository(DatabaseContext dbContext) : IJobRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task Add(Job job)
    {
        var jobEntity = JobMapper.MapJobToEntity(job);
        await _dbContext.AddAsync(jobEntity);
    }

    public void Delete(int id)
    {
        _dbContext.Attach(new Entity.Job { Id = id }).State = EntityState.Deleted;
    }

    public async Task<IList<Job>> GetAll()
    {
        var jobEntities = await _dbContext.Jobs
            .AsNoTracking()
            .Include(j => j.JobRuns)
            .ThenInclude(x => x.Arguments)
            .ToListAsync();

        return jobEntities.Select(JobMapper.MapJobToDomain).ToList();
    }

    public async Task<Job?> GetById(int id)
    {
        var jobEntity = await _dbContext.Jobs
            .AsNoTracking()
            .Include(j => j.JobRuns)
            .ThenInclude(x => x.Arguments)
            .FirstOrDefaultAsync(j => j.Id == id);

        return jobEntity == null ? null : JobMapper.MapJobToDomain(jobEntity);
    }

    public async Task<IList<Job>> GetJobs()
    {
        var jobs = await _dbContext.Jobs
            .AsNoTracking()
            .Include(j => j.JobRuns)
            .ThenInclude(x => x.Arguments)
            .ToListAsync();

        return jobs.Select(JobMapper.MapJobToDomain).ToList();
    }

    public void Update(Job job)
    {
        var jobEntity = JobMapper.MapJobToEntity(job);

        _dbContext.Update(jobEntity);
    }

}
