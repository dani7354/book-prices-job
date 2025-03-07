using BookPricesJob.Application.Contract;
using BookPricesJob.Common.Exception;
using BookPricesJob.Data.Mapper;
using BookPricesJob.Common.Domain;
using Microsoft.EntityFrameworkCore;
using BookPricesJob.Data.DatabaseContext;
using BookPricesJob.Application.Service;

namespace BookPricesJob.Data.Repository;

public class JobRunRepository(DefaultDatabaseContext dbContext) : IJobRunRepository
{
    private static readonly IDictionary<string, int > PriorityEnumValues = new Dictionary<string, int>
    {
        { JobRunPriority.Low.ToString(), (int) JobRunPriority.Low },
        { JobRunPriority.Normal.ToString(), (int) JobRunPriority.Normal },
        { JobRunPriority.High.ToString(), (int) JobRunPriority.High }
    };

    private static readonly IDictionary<string, int> StatusEnumValues = new Dictionary<string, int>
    {
        { JobRunStatus.Running.ToString(), (int) JobRunStatus.Running },
        { JobRunStatus.Pending.ToString(), (int) JobRunStatus.Pending },
        { JobRunStatus.Failed.ToString(), (int) JobRunStatus.Failed },
        { JobRunStatus.Completed.ToString(), (int) JobRunStatus.Completed }
    };
    
    public async Task<string> Add(JobRun jobDomain)
    {
        var newEntity = JobRunMapper.MapToNewEntity(jobDomain);
        await dbContext.JobRun.AddAsync(newEntity);

        return newEntity.Id;
    }

    public async Task Delete(string id)
    {
        var jobRunEntity = await dbContext.JobRun
            .FirstOrDefaultAsync(x => x.Id == id) ??
                throw new NotFoundException(id: id);

        dbContext.JobRun.Remove(jobRunEntity);
    }

    public async Task<IList<JobRun>> FilterBy(
        bool? active,
        int? limit,
        string? jobId, 
        IEnumerable<JobRunStatus>? statuses, 
        IEnumerable<JobRunPriority>? priorities,
        SortByOption sortBy,
        SortDirection sortDirection)
    {
        var query = dbContext.JobRun.AsNoTracking();
        query = ApplyFilter(query, active, jobId, statuses, priorities);
        
        if (limit is not null)
            query = query.Take(limit.Value);
        
        query = query
            .Include(j => j.Arguments)
            .ThenInclude(x => x.Values);
        
        var jobRuns = await query.ToListAsync();
        var jobRunsDomain = ApplySortingAndMapToDomain(jobRuns, sortBy, sortDirection);

        return jobRunsDomain;
    }

    private static IQueryable<Data.Entity.JobRun> ApplyFilter(
        IQueryable<Data.Entity.JobRun> query,
        bool? active, 
        string? jobId, 
        IEnumerable<JobRunStatus>? statuses, 
        IEnumerable<JobRunPriority>? priorities)
    {
        if (jobId is not null)
            query = query.Where(j => j.JobId == jobId);
            
        if (active.HasValue)
            query = query.Where(j => j.Job.IsActive == active);
            
        if (statuses is not null)
        {
            var statusesSet = statuses
                .Select(s => s.ToString())
                .ToList();
            
            query = query.Where(j => statusesSet.Contains(j.Status));
        }

        if (priorities is not null)
        {
            var prioritiesSet = priorities
                .Select(s => s.ToString())
                .ToList();
            
            query = query.Where(j => prioritiesSet.Contains(j.Priority));
        }
        
        return query;
    }
    
    private static IList<JobRun> ApplySortingAndMapToDomain(IEnumerable<Data.Entity.JobRun> query, SortByOption sortBy, SortDirection sortDirection)
    {
        switch (sortBy)
        {
            case SortByOption.Priority when sortDirection == SortDirection.Ascending:
                query = query.OrderBy(x => PriorityEnumValues[x.Priority]);
                break;
            case SortByOption.Priority when sortDirection == SortDirection.Descending:
                query = query.OrderByDescending(x => PriorityEnumValues[x.Priority]);
                break;
            case SortByOption.Status when sortDirection == SortDirection.Ascending:
                query = query.OrderBy(x => StatusEnumValues[x.Status]);
                break;
            case SortByOption.Status when sortDirection == SortDirection.Descending:
                query = query.OrderByDescending(x => StatusEnumValues[x.Status]);
                break;
            case SortByOption.Updated when sortDirection == SortDirection.Ascending:
                query = query.OrderBy(x => x.Updated);
                break;
            case SortByOption.Updated when sortDirection == SortDirection.Descending:
                query = query.OrderByDescending(x => x.Updated);
                break;
        }

        return query
            .Select(JobRunMapper.MapToDomain)
            .ToList();
    }

    public async Task<IList<JobRun>> GetAll()
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

    public async Task<JobRun?> GetById(string id)
    {
        var jobRun = await dbContext.JobRun
            .AsNoTracking()
            .Include(j => j.Arguments)
                .ThenInclude(x => x.Values)
            .FirstOrDefaultAsync(j => j.Id == id);

        return jobRun is null ? null : JobRunMapper.MapToDomain(jobRun);
    }

    public async Task Update(JobRun jobRunDomain)
    {
        var existingEntity = await dbContext.JobRun
            .Include(j => j.Arguments)
            .FirstOrDefaultAsync(x => x.Id == jobRunDomain.Id)
            ?? throw new NotFoundException(id: jobRunDomain.Id!);

        dbContext.JobRunArgument.RemoveRange(existingEntity.Arguments);
        var updatedJobRun = JobRunMapper.MapToEntity(jobRunDomain, existingEntity);
        
        dbContext.Entry(existingEntity).Property(x => x.Version).OriginalValue = jobRunDomain.Version;
        dbContext.JobRun.Update(updatedJobRun);
    }
}
