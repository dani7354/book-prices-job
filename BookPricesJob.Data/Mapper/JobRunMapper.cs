using BookPricesJob.Common.Domain;
using BookPricesJob.Data.Entity;

namespace BookPricesJob.Application.Mapper;

public static class JobRunMapper
{
    public static Data.Entity.JobRun MapToNewEntity(Common.Domain.JobRun jobRunDomain)
    {
        var entity = new Data.Entity.JobRun();
        entity.Id = Guid.NewGuid().ToString();
        entity.JobId = jobRunDomain.JobId;
        entity.Status = jobRunDomain.Status.ToString();
        entity.Priority = jobRunDomain.Priority.ToString();
        entity.Created = DateTime.UtcNow;
        entity.Updated = DateTime.UtcNow;

        var arguments = jobRunDomain.Arguments.Select(x => new Data.Entity.JobRunArgument
            {
                JobRunId = Guid.NewGuid().ToString(),
                Name = x.Name,
                Values = x.Values.Select(
                    v => new JobRunArgumentValue { Id = Guid.NewGuid().ToString(), Value = v }).ToList()
            }).ToList();

        entity.Arguments.AddRange(arguments);

        return entity;
    }

    public static Data.Entity.JobRun MapToEntity(Common.Domain.JobRun jobRunDomain, Data.Entity.JobRun jobRunEntity)
    {
        jobRunEntity.Id = jobRunDomain.Id!;
        jobRunEntity.JobId = jobRunDomain.JobId;
        jobRunEntity.Status = jobRunDomain.Status.ToString();
        jobRunEntity.Updated = DateTime.UtcNow;
        jobRunEntity.ErrorMessage = jobRunDomain.ErrorMessage;

        var arguments = jobRunDomain.Arguments.Select(x => new Data.Entity.JobRunArgument
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

    public static Common.Domain.JobRun MapToDomain(Data.Entity.JobRun jobRunEntity)
    {
        return new Common.Domain.JobRun(
            jobRunEntity.Id,
            jobRunEntity.JobId,
            jobRunEntity.Created,
            jobRunEntity.Updated,
            Enum.Parse<JobRunStatus>(jobRunEntity.Status),
            Enum.Parse<JobRunPriority>(jobRunEntity.Priority),
            jobRunEntity.Arguments.Select(
                x => new Common.Domain.JobRunArgument(x.Id, x.Name, x.Type, x.Values.Select(v => v.Value).ToArray())).ToList(),
            jobRunEntity.ErrorMessage
        );
    }
}
