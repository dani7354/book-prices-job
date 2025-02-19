using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.API.Mapper;

public static class JobRunMapper
{
    public static JobRun MapToDomain(CreateJobRunRequest createRequest)
    {
        return new JobRun(
            Id: null,
            JobId: createRequest.JobId,
            Created: DateTime.UtcNow,
            Updated: DateTime.UtcNow,
            Status: JobRunStatus.Pending,
            Priority: Enum.Parse<JobRunPriority>(createRequest.Priority),
            Arguments: createRequest.Arguments
                .Select(x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values))
                .ToList(),
            ErrorMessage: null
        );
    }

    public static JobRun MapToDomain(
        UpdateJobRunFullRequest updateRequest,
        JobRun jobRun)
    {
        return new JobRun(
            Id: jobRun.Id,
            JobId: jobRun.JobId,
            Created: jobRun.Created,
            Updated: DateTime.UtcNow,
            Status: Enum.Parse<JobRunStatus>(updateRequest.Status),
            Priority: Enum.Parse<JobRunPriority>(updateRequest.Priority),
            Arguments: updateRequest.Arguments
                .Select(x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values))
                .ToList(),
            ErrorMessage: updateRequest.ErrorMessage
        );
    }

    public static IList<JobRunListItemDto> MapToListDto(IList<(JobRun, Job)> jobRuns)
    {
        return jobRuns
            .Select(x => MapToListItemDto(x.Item1, x.Item2.Name))
            .ToList();
    }

    public static JobRunListItemDto MapToListItemDto(JobRun jobRun, string jobName)
    {
        return new JobRunListItemDto(
            jobRun.Id!,
            jobRun.JobId,
            jobName,
            jobRun.Priority.ToString(),
            jobRun.Status.ToString(),
            jobRun.Created,
            jobRun.Updated,
            jobRun.Arguments
                .Select(
                    x => new JobRunArgumentDto { Name = x.Name, Type = x.Type, Values = x.Values })
                .ToList()
        );
    }

    public static JobRunDto MapToDto(JobRun jobRun, string jobName)
    {
        return new JobRunDto(
            jobRun.Id!,
            jobRun.JobId,
            jobName,
            jobRun.Priority.ToString(),
            jobRun.Status.ToString(),
            jobRun.Created,
            jobRun.Updated,
            jobRun.Arguments.Select(
                    x => new JobRunArgumentDto() { Name = x.Name, Type = x.Type, Values = x.Values })
                .ToList(),
            jobRun.ErrorMessage
        );
    }
}
