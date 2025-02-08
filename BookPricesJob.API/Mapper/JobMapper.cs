using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.API.Mapper;

public static class JobMapper
{
    public static Job MapToDomain(CreateJobRequest createRequest)
    {
        return new Job(
            Id: null,
            IsActive: createRequest.IsActive ?? false,
            Name: createRequest.Name,
            Description: createRequest.Description,
            Created: null,
            JobRuns: []
        );
    }

    public static Job MapToDomain(UpdateJobFullRequest updateRequest, Job job)
    {
        return job with 
            { 
                IsActive = updateRequest.IsActive ?? false, 
                Name = updateRequest.Name, 
                Description = updateRequest.Description 
            };
    }

    public static JobListItemDto MapToListItemDto(Job job)
    {
        return new JobListItemDto(
            job.IsActive,
            job.Id!,
            job.Name,
            job.Description,
            job.Created!.Value
        );
    }

    public static JobDto MapToDto(Job job)
    {
        return new JobDto(
            Id: job.Id!,
            IsActive: job.IsActive,
            Name: job.Name,
            Description: job.Description,
            Created: job.Created!.Value,
            JobRuns: job.JobRuns
                .Select(x => JobRunMapper.MapToListItemDto(x, job.Name))
                .ToList()
        );
    }

    public static IList<JobListItemDto> MapToList(IList<Job> jobs)
    {
        return jobs.Select(MapToListItemDto).ToList();
    }
}
