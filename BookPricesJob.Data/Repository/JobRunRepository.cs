using System.Collections.Generic;
using System.Threading.Tasks;
using BookPricesJob.Application.Contract;
using BookPricesJob.Application.Mapper;
using BookPricesJob.Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace BookPricesJob.Data.Repository;

public class JobRunRepository(DatabaseContext dbContext) : IJobRunRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<string> Add(JobRun jobDomain)
    {
        var newEntity = JobRunMapper.MapToNewEntity(jobDomain);
        await _dbContext.JobRun.AddAsync(newEntity);

        return newEntity.Id.ToString();
    }

    public async Task Delete(string id)
    {
        var jobRunEntity = await _dbContext.Job
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (jobRunEntity is null) return;
        _dbContext.Job.Remove(jobRunEntity);
    }

    public async Task<IList<JobRun>> GetAll()
    {
        var jobRuns = await _dbContext.JobRun
            .AsNoTracking()
            .Include(j => j.Arguments)
                .ThenInclude(x => x.Values)
            .ToListAsync();

        return jobRuns.Select(JobRunMapper.MapToDomain).ToList();
    }

    public async Task<JobRun?> GetById(string id)
    {
        var jobRun = await _dbContext.JobRun
            .AsNoTracking()
            .Include(j => j.Arguments)
                .ThenInclude(x => x.Values)
            .FirstOrDefaultAsync(j => j.Id == id);

        return jobRun is null ? null : JobRunMapper.MapToDomain(jobRun);
    }

    public async Task Update(JobRun jobRunDomain)
    {
        var existingEntity = await _dbContext.JobRun.FirstOrDefaultAsync(x => x.Id == jobRunDomain.Id);
        if (existingEntity is null)
            throw new KeyNotFoundException($"JobRun with id {jobRunDomain.Id} not found");

        var updatedJobRun = JobRunMapper.MapToEntity(jobRunDomain, existingEntity);

        _dbContext.JobRun.Update(updatedJobRun);
    }
}
