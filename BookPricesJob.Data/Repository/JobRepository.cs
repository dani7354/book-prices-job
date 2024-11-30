using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Domain;
using BookPricesJob.Data.Mapper;
using BookPricesJob.Common.Exception;
using MySqlConnector;
using BookPricesJob.Application.DatabaseContext;


namespace BookPricesJob.Data.Repository;

public class JobRepository(DatabaseContextBase dbContext) : IJobRepository
{
    private readonly DatabaseContextBase _dbContext = dbContext;

    public async Task<string> Add(Job job)
    {
        try
        {
            var jobEntity = JobMapper.MapJobToEntity(job);
            await _dbContext.AddAsync(jobEntity);

            return jobEntity.Id.ToString();
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task Delete(string id)
    {
        try
        {
            var jobEntity = await _dbContext.Job
                .FirstOrDefaultAsync(x => x.Id == id) ??
                throw new NotFoundException(id: id);

            _dbContext.Job.Remove(jobEntity);
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<IList<Job>> GetAll()
    {
        try
        {
            var jobEntities = await _dbContext.Job
                .AsNoTracking()
                .Include(j => j.JobRuns)
                    .ThenInclude(x => x.Arguments)
                .ToListAsync();

            return jobEntities.Select(JobMapper.MapJobToDomain).ToList();
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<Job?> GetById(string id)
    {
        try
        {
            var jobEntity = await _dbContext.Job
                .AsNoTracking()
                .Include(j => j.JobRuns)
                    .ThenInclude(x => x.Arguments)
                .FirstOrDefaultAsync(j => j.Id == id);

            return jobEntity == null ? null : JobMapper.MapJobToDomain(jobEntity);
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<IList<Job>> GetJobs()
    {
        try
        {
            var jobs = await _dbContext.Job
                .AsNoTracking()
                .Include(j => j.JobRuns)
                    .ThenInclude(x => x.Arguments)
                .ToListAsync();

            return jobs.Select(JobMapper.MapJobToDomain).ToList();
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task Update(Job job)
    {
        try
        {
            var existingEntity = await _dbContext.Job.FirstOrDefaultAsync(x => x.Id == job.Id)
             ?? throw new NotFoundException(id: job.Id!);
            var jobEntity = JobMapper.MapJobToEntity(job, existingEntity);

            _dbContext.Update(jobEntity);
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new UpdateFailedException(e.Message);
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }
}
