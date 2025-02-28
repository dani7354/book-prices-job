using BookPricesJob.Common.Domain;

namespace BookPricesJob.Data.Mapper;

public static class JobMapper
{
    private const int MaxJobRunsToInclude = 50;

    public static Job MapJobToDomain(Entity.Job job)
    {
        return new Job(
            job.Id,
            job.IsActive,
            job.Name,
            job.Description,
            job.Version,
            job.Created,
            job.JobRuns
                .OrderByDescending(x => x.Updated)
                .Take(MaxJobRunsToInclude)
                .Select(JobRunMapper.MapToDomain)
                .ToList()
        );
    }

    public static Entity.Job MapJobToEntity(Job jobUpdated, Entity.Job? jobEntity = null)
    {
        jobEntity ??= new Entity.Job();
        jobEntity.Id = jobUpdated.Id ?? Guid.NewGuid().ToString();
        jobEntity.IsActive = jobUpdated.IsActive;
        jobEntity.Name = jobUpdated.Name;
        jobEntity.Description = jobUpdated.Description;
        jobEntity.Version = Guid.NewGuid().ToString();
        jobEntity.Created = jobUpdated.Created ?? DateTime.Now;

        return jobEntity;
    }
}
