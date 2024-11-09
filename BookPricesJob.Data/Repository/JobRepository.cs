using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using BookPricesJob.Data.Mapper;
using BookPricesJob.Common.Exception;


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
            .FirstOrDefaultAsync(x => x.Id == id) ??
                throw new JobNotFoundException(id: id);

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
        try
        {
            var existingEntity = await _dbContext.Job.FirstOrDefaultAsync(x => x.Id == job.Id)
             ?? throw new JobNotFoundException("Job with id {job.Id} not found!");
            var jobEntity = JobMapper.MapJobToEntity(job, existingEntity);

            _dbContext.Update(jobEntity);
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new UpdateFailedException(e.Message);
        }
    }
}
