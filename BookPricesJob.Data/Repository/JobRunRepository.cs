using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Exception;
using BookPricesJob.Application.Mapper;
using BookPricesJob.Common.Domain;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using BookPricesJob.Application.DatabaseContext;

namespace BookPricesJob.Data.Repository;

public class JobRunRepository(DatabaseContextBase dbContext) : IJobRunRepository
{
    public async Task<string> Add(JobRun jobDomain)
    {
        try
        {
            var newEntity = JobRunMapper.MapToNewEntity(jobDomain);
            await dbContext.JobRun.AddAsync(newEntity);

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
            var jobRunEntity = await dbContext.JobRun
                .FirstOrDefaultAsync(x => x.Id == id) ??
                    throw new NotFoundException(id: id);

            dbContext.JobRun.Remove(jobRunEntity);
        }
        catch (MySqlException e)
        {
            throw new DatabaseException(e);
        }
    }

    public async Task<IList<JobRun>> FilterBy(
        bool? active,
        int? limit,
        string? jobId, 
        IEnumerable<JobRunStatus>? statuses, 
        IEnumerable<JobRunPriority>? priorities)
    {
        try
        {
            var query = dbContext.JobRun.AsNoTracking();
            if (jobId is not null)
                query = query.Where(j => j.JobId == jobId);
            
            if (active.HasValue)
                query = query.Where(j => j.Job.IsActive == active);
            
            if (statuses is not null)
            {
                var statusesSet = statuses.Select(s => s.ToString()).ToHashSet();
                query = query.Where(j => statusesSet.Contains(j.Status));
            }

            if (priorities is not null)
            {
                var prioritiesSet = priorities.Select(s => s.ToString()).ToHashSet();
                query = query.Where(j => prioritiesSet.Contains(j.Priority));
            }
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
            var jobRuns = await dbContext.JobRun
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
            var jobRun = await dbContext.JobRun
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
            var existingEntity = await dbContext.JobRun
                .Include(j => j.Arguments)
                .FirstOrDefaultAsync(x => x.Id == jobRunDomain.Id)
                ?? throw new NotFoundException(id: jobRunDomain.Id!);

            dbContext.JobRunArgument.RemoveRange(existingEntity.Arguments);
            var updatedJobRun = JobRunMapper.MapToEntity(jobRunDomain, existingEntity);

            dbContext.JobRun.Update(updatedJobRun);

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
