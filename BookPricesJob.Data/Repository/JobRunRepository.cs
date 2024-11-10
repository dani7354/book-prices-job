using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Exception;
using BookPricesJob.Application.Mapper;
using BookPricesJob.Common.Domain;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace BookPricesJob.Data.Repository;

public class JobRunRepository(DatabaseContext dbContext) : IJobRunRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<string> Add(JobRun jobDomain)
    {
        try
        {
            var newEntity = JobRunMapper.MapToNewEntity(jobDomain);
            await _dbContext.JobRun.AddAsync(newEntity);

            return newEntity.Id.ToString();
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
            var jobRunEntity = await _dbContext.JobRun
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id) ??
                    throw new NotFoundException(id: id);

            _dbContext.JobRun.Remove(jobRunEntity);
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<IList<JobRun>> FilterBy(string? jobId, JobRunStatus? status, JobRunPriority? priority, int? limit)
    {
        try
        {
            var query = _dbContext.JobRun.AsNoTracking();

            if (jobId is not null)
                query = query.Where(j => j.JobId == jobId);
            if (status is not null)
                query = query.Where(j => j.Status == status.Value.ToString());
            if (priority is not null)
                query = query.Where(j => j.Priority == priority.Value.ToString());
            if (limit is not null)
                query = query.Take(limit.Value);

            var jobRuns = await query
                .Include(j => j.Arguments)
                    .ThenInclude(x => x.Values)
                .OrderByDescending(j => j.Priority)
                    .ThenBy(j => j.Updated)
                .ToListAsync();

            return jobRuns.Select(JobRunMapper.MapToDomain).ToList();
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<IList<JobRun>> GetAll()
    {
        try
        {
            var jobRuns = await _dbContext.JobRun
                .AsNoTracking()
                .Include(j => j.Arguments)
                    .ThenInclude(x => x.Values)
                .OrderByDescending(j => j.Priority)
                    .ThenBy(j => j.Updated)
                .ToListAsync();

            return jobRuns.Select(JobRunMapper.MapToDomain).ToList();
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<JobRun?> GetById(string id)
    {
        try
        {
            var jobRun = await _dbContext.JobRun
                .AsNoTracking()
                .Include(j => j.Arguments)
                    .ThenInclude(x => x.Values)
                .FirstOrDefaultAsync(j => j.Id == id);

            return jobRun is null ? null : JobRunMapper.MapToDomain(jobRun);
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task Update(JobRun jobRunDomain)
    {
        try
        {
            var existingEntity = await _dbContext.JobRun
                .Include(j => j.Arguments)
                .FirstOrDefaultAsync(x => x.Id == jobRunDomain.Id)
                ?? throw new NotFoundException(id: jobRunDomain.Id!);

            _dbContext.JobRunArgument.RemoveRange(existingEntity.Arguments);
            var updatedJobRun = JobRunMapper.MapToEntity(jobRunDomain, existingEntity);

            _dbContext.JobRun.Update(updatedJobRun);

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
