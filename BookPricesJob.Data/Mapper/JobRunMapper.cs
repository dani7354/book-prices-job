using BookPricesJob.Common.Domain;
using BookPricesJob.Data.Entity;

namespace BookPricesJob.Data.Mapper;

public static class JobRunMapper
{
    public static Entity.JobRun MapToNewEntity(Common.Domain.JobRun jobRunDomain)
    {
        var entity = new Entity.JobRun
        {
            Id = Guid.NewGuid().ToString(),
            JobId = jobRunDomain.JobId,
            Status = jobRunDomain.Status.ToString(),
            Priority = jobRunDomain.Priority.ToString(),
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            Version = jobRunDomain.Version
        };

        var arguments = jobRunDomain.Arguments.Select(x => new Entity.JobRunArgument
            {
                Id = Guid.NewGuid().ToString(),
                Name = x.Name,
                Type = x.Type,
                Values = x.Values.Select(
                    v => new JobRunArgumentValue { Id = Guid.NewGuid().ToString(), Value = v }).ToList()
            }).ToList();

        entity.Arguments.AddRange(arguments);

        return entity;
    }

    public static Entity.JobRun MapToEntity(Common.Domain.JobRun jobRunDomain, Entity.JobRun jobRunEntity)
    {
        jobRunEntity.Id = jobRunDomain.Id!;
        jobRunEntity.JobId = jobRunDomain.JobId;
        jobRunEntity.Status = jobRunDomain.Status.ToString();
        jobRunEntity.Priority = jobRunDomain.Priority.ToString();
        jobRunEntity.Updated = DateTime.UtcNow;
        jobRunEntity.ErrorMessage = jobRunDomain.ErrorMessage;
        jobRunEntity.Version = jobRunDomain.Version;

        var arguments = jobRunDomain.Arguments.Select(x => new Entity.JobRunArgument
            {
                Id = Guid.NewGuid().ToString(),
                JobRunId = jobRunDomain.Id!,
                Name = x.Name,
                Type = x.Type,
                Values = x.Values.Select(v => new JobRunArgumentValue { Id = Guid.NewGuid().ToString(), Value = v }).ToList()
            }).ToList();

        jobRunEntity.Arguments.Clear();
        jobRunEntity.Arguments.AddRange(arguments);

        return jobRunEntity;
    }

    public static Common.Domain.JobRun MapToDomain(Entity.JobRun jobRunEntity)
    {
        return new Common.Domain.JobRun(
            jobRunEntity.Id,
            jobRunEntity.JobId,
            jobRunEntity.Created,
            jobRunEntity.Updated,
            Enum.Parse<JobRunStatus>(jobRunEntity.Status),
            Enum.Parse<JobRunPriority>(jobRunEntity.Priority),
            jobRunEntity.Arguments.Select(
                    x => new Common.Domain.JobRunArgument(x.Id, x.Name, x.Type,
                        x.Values.Select(v => v.Value).ToArray()))
                .ToList(),
            jobRunEntity.ErrorMessage,
            jobRunEntity.Version);
    }
}
