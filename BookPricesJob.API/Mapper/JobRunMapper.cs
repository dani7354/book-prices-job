using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.API.Mapper;

public static class JobRunMapper
{
    public static JobRun MapToDomain(CreateJobRunDto jobRunDto)
    {
        return new JobRun(
            Id: null,
            JobId: jobRunDto.JobId,
            Created: DateTime.UtcNow,
            Updated: DateTime.UtcNow,
            Status: JobRunStatus.Pending,
            Priority: Enum.Parse<JobRunPriority>(jobRunDto.Priority),
            Arguments: jobRunDto.Arguments.Select(x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values)).ToList(),
            ErrorMessage: null
        );
    }

    public static JobRun MapToDomain(UpdateJobRunFullDto jobRunDto, JobRun jobRun)
    {
        return new JobRun(
            Id: jobRun.Id,
            JobId: jobRun.JobId,
            Created: jobRun.Created,
            Updated: DateTime.UtcNow,
            Status: Enum.Parse<JobRunStatus>(jobRunDto.Status),
            Priority: Enum.Parse<JobRunPriority>(jobRunDto.Priority),
            Arguments: jobRunDto.Arguments.Select(x => new JobRunArgument(Id: null, x.Name, x.Type, x.Values)).ToList(),
            ErrorMessage: jobRunDto.ErrorMessage
        );
    }

    public static IList<JobRunListItemDto> MapToListDto(IList<JobRun> jobRuns)
    {
        return jobRuns.Select(
            x => new JobRunListItemDto(
                x.Id!,
                x.JobId,
                x.Priority.ToString(),
                x.Status.ToString(),
                x.Created,
                x.Updated)).ToList();
    }

    public static JobRunDto MapToDto(JobRun jobRun)
    {
        return new JobRunDto(
            jobRun.Id!,
            jobRun.JobId,
            jobRun.Priority.ToString(),
            jobRun.Status.ToString(),
            jobRun.Created,
            jobRun.Updated,
            jobRun.Arguments.Select(
                x => new JobRunArgumentDto() { Name = x.Name, Type = x.Type, Values = x.Values }).ToList(),
            jobRun.ErrorMessage
        );
    }
}
