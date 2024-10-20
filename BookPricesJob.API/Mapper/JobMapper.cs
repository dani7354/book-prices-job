using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.API.Mapper;

public static class JobMapper
{
    public static Job MapToDomain(CreateJobDto jobDto)
    {
        return new Job(
            Id: null,
            IsActive: jobDto.IsActive,
            Name: jobDto.Name,
            Description: jobDto.Description,
            Created: null,
            JobRuns: []
        );
    }

    public static Job MapToDomain(UpdateJobFullDto jobDto, Job job)
    {
        return new Job(
            Id: job.Id,
            IsActive: jobDto.IsActive,
            Name: jobDto.Name,
            Description: jobDto.Description,
            Created: job.Created,
            JobRuns: job.JobRuns
        );
    }

    public static JobListItemDto MapToDto(Job job)
    {
        return new JobListItemDto(
            job.IsActive,
            job.Id!,
            job.Name,
            job.Description,
            job.Created!.Value
        );
    }

    public static IList<JobListItemDto> MapToList(IList<Job> jobs)
    {
        return jobs.Select(MapToDto).ToList();
    }
}
