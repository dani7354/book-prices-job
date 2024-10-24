using BookPricesJob.Application.Mapper;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Data.Mapper;

public class JobMapper
{
    public static Job MapJobToDomain(Entity.Job job)
    {
        return new Job(
            job.Id,
            job.IsActive,
            job.Name,
            job.Description,
            job.Created,
            job.JobRuns.Select(JobRunMapper.MapToDomain).ToList()
        );
    }

    public static Entity.Job MapJobToEntity(Job jobUpdated, Entity.Job? jobEntity = null)
    {
        jobEntity ??= new Entity.Job();
        jobEntity.Id = jobUpdated.Id ?? Guid.NewGuid().ToString();
        jobEntity.IsActive = jobUpdated.IsActive;
        jobEntity.Name = jobUpdated.Name;
        jobEntity.Description = jobUpdated.Description;
        jobEntity.Created = jobUpdated.Created ?? DateTime.Now;

        return jobEntity;
    }
}
