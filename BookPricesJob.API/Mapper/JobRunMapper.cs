using BookPricesJob.API.Extension;
using BookPricesJob.API.Model;
using BookPricesJob.Application.Service;
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
            ErrorMessage: null,
            Version: Guid.NewGuid().ToString());
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
            ErrorMessage: updateRequest.ErrorMessage,
            Version: updateRequest.Version
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
            jobRun.Version!,
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
            jobRun.Version!,
            jobRun.Created,
            jobRun.Updated,
            jobRun.Arguments.Select(
                    x => new JobRunArgumentDto() { Name = x.Name, Type = x.Type, Values = x.Values })
                .ToList(),
            jobRun.ErrorMessage
        );
    }

    public static JobRunFilter MapFilterRequestToFilter(JobRunListRequest listRequest)
    {
        var jobRunStatuses = listRequest.Status?
            .Select(s => s.SafelyConvertToEnum<JobRunStatus>())
            .Where(p => p != null)
            .Select(p => p!.Value);

        var jobRunPriorities = listRequest.Priority?
            .Select(p => p.SafelyConvertToEnum<JobRunPriority>())
            .Where(p => p != null)
            .Select(p => p!.Value);

        var sortBy = listRequest.SortBy?.SafelyConvertToEnum<SortByOption>() ?? SortByOption.Updated;
        var sortDirection = listRequest.SortDirection?.SafelyConvertToEnum<SortDirection>() ?? SortDirection.Ascending;
        
        return new JobRunFilter(
            listRequest.Active,
            listRequest.Limit,
            listRequest.JobId,
            jobRunPriorities,
            jobRunStatuses,
            sortBy,
            sortDirection);
    }
}
