using BookPricesJob.API.Model;

namespace BookPricesJob.Test.Setup;

public static class TestData
{
    private const string JobName = "TestJob_1";
    private const string JobDescription = "TestJob_1_Description";
    private const bool IsActive = true;

    public static CreateJobRequest GetCreateJobRequest(
        string name = JobName,
        string description = JobDescription,
        bool isActive = IsActive)
    {
        return new CreateJobRequest
        {
            Name = JobName,
            Description = description,
            IsActive = isActive
        };
    }

    public static UpdateJobPartialRequest GetUpdatePartialRequest(
        string id,
        string version,
        string? name = null,
        string? description = null,
        bool? isActive = null)
    {
        return new UpdateJobPartialRequest
        {
            Id = id,
            Version = version,
            Name = name,
            Description = description,
            IsActive = isActive
        };
    }
    
    public static UpdateJobFullRequest GetUpdateJobFullRequest(
        string id,
        string version,
        string name = JobName,
        string description = JobDescription,
        bool isActive = IsActive)
    {
        return new UpdateJobFullRequest
        {
            Id = id,
            Name = name,
            Description = description,
            Version = version,
            IsActive = isActive
        };
    }
}
