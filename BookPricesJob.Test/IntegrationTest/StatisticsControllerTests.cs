using System.Net;
using System.Net.Http.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;
using BookPricesJob.Test.Setup;

namespace BookPricesJob.Test.IntegrationTest;

public class StatisticsControllerTests
{
    private readonly HttpClient _client;

    public StatisticsControllerTests()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        var factory = new CustomWebApplicationFactory<Startup>();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FinishedJobRuns_NoData_ReturnsSuccessWithEmptyJobRunsList()
    {
        const int days = 15;
        var response = await _client.GetAsync(GetFinishedJobRunStatsUrl(days));

        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<FinishedJobRunsStatisticsDto>();
        Assert.NotNull(dto);
        Assert.NotNull(dto.GeneratedAt);
        Assert.Empty(dto.JobRuns);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal(Constant.ContentTypeValue, contentType);
    }

    [Fact]
    public async Task FinishedJobRuns_WithJobAndJobRun_ReturnsCorrectCounts()
    {
        const int completedCount = 6, failedCount = 6;
        await CreateJobAndJobRuns(_client, completedCount: completedCount, failedCount: failedCount);

        const int days = 15;
        var response = await _client.GetAsync(GetFinishedJobRunStatsUrl(days));
        response.EnsureSuccessStatusCode();

        var statisticsDto = await response.Content.ReadFromJsonAsync<FinishedJobRunsStatisticsDto>();
        Assert.NotNull(statisticsDto);
        Assert.NotEmpty(statisticsDto.JobRuns);

        var jobRunCount = statisticsDto.JobRuns.FirstOrDefault();
        Assert.NotNull(jobRunCount);
        
        Assert.Equal(completedCount + failedCount, statisticsDto.JobRuns.First().TotalJobRunCount);
        Assert.Equal(completedCount, statisticsDto.JobRuns.First().JobRunCountByStatus[nameof(JobRunStatus.Completed)]);
        Assert.Equal(50, statisticsDto.JobRuns.First().JobRunPercentageByStatus[nameof(JobRunStatus.Completed)]);
    }

    private static async Task CreateJobAndJobRuns(HttpClient client, int completedCount, int failedCount)
    {
        var jobPayload = TestData.GetCreateJobRequest();
        var jobContent = HttpClientHelper.CreateStringPayload(jobPayload);
        var createJobResponse = await client.PostAsync(Constant.JobsBaseEndpoint, jobContent);
        Assert.Equal(HttpStatusCode.Created, createJobResponse.StatusCode);

        var createdJob = await createJobResponse.Content.ReadFromJsonAsync<JobDto>();
        Assert.NotNull(createdJob);

        for (var i = 0; i < completedCount; i++)
            await CreateJobRunForJobAndSetStatus(client, createdJob.Id, nameof(JobRunStatus.Completed));

        for (var i = 0; i < failedCount; i++)
            await CreateJobRunForJobAndSetStatus(client, createdJob.Id, nameof(JobRunStatus.Failed));
    }

    private static async Task CreateJobRunForJobAndSetStatus(
        HttpClient client, 
        string jobId, 
        string status)
    {
        var jobRunDto = await HttpClientHelper.CreateJobRunForJob(client, jobId);
        
        var updateJobRunRequest = new UpdateJobRunPartialRequest
        {
            JobId = jobId,
            JobRunId = jobRunDto.Id,
            Status = status,
            Version = jobRunDto.Version
        };
        
        var updateJobRunResponse = await HttpClientHelper.PatchJobRun(client, updateJobRunRequest);
        updateJobRunResponse.EnsureSuccessStatusCode();
    }

    private static string GetFinishedJobRunStatsUrl(int days)
        => $"{Constant.FinishedJobRunsEndpoint}?days={days}";
}