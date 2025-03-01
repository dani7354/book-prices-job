using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using BookPricesJob.Data.Mapper;
using BookPricesJob.Common.Exception;
using BookPricesJob.Data.DatabaseContext;


namespace BookPricesJob.Data.Repository;

public class JobRepository(DatabaseContextBase dbContext) : IJobRepository
{
    public async Task<string> Add(Job job)
    {
        var jobEntity = JobMapper.MapJobToEntity(job);
        await dbContext.AddAsync(jobEntity);

        return jobEntity.Id;
    }

    public async Task Delete(string id)
    {
        var jobEntity = await dbContext.Job
            .FirstOrDefaultAsync(x => x.Id == id) ??
            throw new NotFoundException(id: id);

        dbContext.Job.Remove(jobEntity);
    }

    public async Task<IList<Job>> GetAll()
    {
        var jobEntities = await dbContext.Job
            .AsNoTracking()
            .Include(j => j.JobRuns)
                .ThenInclude(x => x.Arguments)
            .ToListAsync();

        return jobEntities.Select(JobMapper.MapJobToDomain).ToList();
    }

    public async Task<Job?> GetById(string id)
    {
        var jobEntity = await dbContext.Job
            .AsNoTracking()
            .Include(j => j.JobRuns)
                .ThenInclude(x => x.Arguments)
            .FirstOrDefaultAsync(j => j.Id == id);

        return jobEntity == null ? null : JobMapper.MapJobToDomain(jobEntity);
    }

    public async Task<IList<Job>> GetJobs()
    {
        var jobs = await dbContext.Job
            .AsNoTracking()
            .Include(j => j.JobRuns)
                .ThenInclude(x => x.Arguments)
            .ToListAsync();

        return jobs.Select(JobMapper.MapJobToDomain).ToList();
    }

    public async Task Update(Job job)
    {
        var existingEntity = await dbContext.Job.FirstOrDefaultAsync(x => x.Id == job.Id)
         ?? throw new NotFoundException(id: job.Id!);
        var jobEntity = JobMapper.MapJobToEntity(job, existingEntity);

        dbContext.Entry(existingEntity).Property(x => x.Version).OriginalValue = job.Version;
        dbContext.Update(jobEntity);
    }
}
