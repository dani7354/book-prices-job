using BookPricesJob.Common.Domain;

namespace BookPricesJob.Data.Mapper;

public class JobMapper
{
    public static Common.Domain.JobRun MapJobRunToDomain(Entity.JobRun jobRun)
    {
        return new Common.Domain.JobRun(
            jobRun.Id,
            jobRun.Created,
            jobRun.Updated,
            Enum.Parse<JobRunStatus>(jobRun.Status),
            jobRun.Arguments.Select(x => new JobRunArgument(x.Id, x.Name, x.Type, x.Values.Select(z => z.Value).ToArray())).ToList(),
            jobRun.ErrorMessage
        );
    }

    public static Entity.JobRun MapJobRunToEntity(Common.Domain.JobRun jobRunUpdated, Entity.JobRun? jobRunEntity = null)
    {
        jobRunEntity ??= new Entity.JobRun();

        jobRunEntity.Id = jobRunUpdated.Id;
        jobRunEntity.Created = jobRunUpdated.Created;
        jobRunEntity.Updated = jobRunUpdated.Updated;
        jobRunEntity.Status = jobRunUpdated.Status.ToString();
        jobRunEntity.ErrorMessage = jobRunUpdated.ErrorMessage;

        return jobRunEntity;
    }

    public static Common.Domain.Job MapJobToDomain(Entity.Job job)
    {
        return new Common.Domain.Job(
            job.Id,
            job.IsActive,
            job.Name,
            job.Description,
            job.Created,
            job.JobRuns.Select(MapJobRunToDomain).ToList()
        );
    }

    public static Entity.Job MapJobToEntity(Common.Domain.Job jobUpdated, Entity.Job? jobEntity = null)
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
