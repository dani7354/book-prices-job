using BookPricesJob.API.Model;

namespace BookPricesJob.Test.Setup;

public static class TestData
{
    private const string JobName = "TestJob_1";
    private const string JobDescription = "TestJob_1_Description";
    private const bool IsActive = true;
    private const bool RunsWithArguments = false;

    public static CreateJobRequest GetCreateJobRequest(
        string name = JobName,
        string description = JobDescription,
        bool isActive = IsActive,
        bool runsWithArguments = RunsWithArguments)
    {
        return new CreateJobRequest
        {
            Name = name,
            Description = description,
            IsActive = isActive,
            RunsWithArguments = runsWithArguments
        };
    }

    public static UpdateJobPartialRequest GetUpdatePartialRequest(
        string id,
        string version,
        string? name = null,
        string? description = null,
        bool? isActive = null,
        bool? runsWithArguments = null)
    {
        return new UpdateJobPartialRequest
        {
            Id = id,
            Version = version,
            Name = name,
            Description = description,
            IsActive = isActive,
            RunsWithArguments = runsWithArguments
        };
    }
    
    public static CreateJobRunRequest GetCreateJobRunRequest(
        string jobId,
        string priority = "Normal",
        List<JobRunArgumentDto>? arguments = null)
    {
        return new CreateJobRunRequest
        {
            JobId = jobId,
            Priority = priority,
            Arguments = arguments ?? []
        };
    }

    public static UpdateJobFullRequest GetUpdateJobFullRequest(
        string id,
        string version,
        string name = JobName,
        string description = JobDescription,
        bool isActive = IsActive,
        bool runsWithArguments = RunsWithArguments)
    {
        return new UpdateJobFullRequest
        {
            Id = id,
            Name = name,
            Description = description,
            Version = version,
            IsActive = isActive,
            RunsWithArguments = runsWithArguments
        };
    }
}
