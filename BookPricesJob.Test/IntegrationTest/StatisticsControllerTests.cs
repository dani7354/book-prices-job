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
        var response = await _client.GetAsync(Constant.FinishedJobRunsEndpoint);

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
        var jobPayload = TestData.GetCreateJobRequest();
        var jobContent = HttpClientHelper.CreateStringPayload(jobPayload);
        var createJobResponse = await _client.PostAsync(Constant.JobsBaseEndpoint, jobContent);
        Assert.Equal(HttpStatusCode.Created, createJobResponse.StatusCode);

        var createdJob = await createJobResponse.Content.ReadFromJsonAsync<JobDto>();
        Assert.NotNull(createdJob);

        var jobRunDto = await HttpClientHelper.CreateJobRunForJob(_client, createdJob.Id, JobRunPriority.High);
        
        var updateJobRunRequest = new UpdateJobRunPartialRequest()
        {
            JobId = createdJob.Id,
            JobRunId = jobRunDto.Id,
            Status = nameof(JobRunStatus.Completed),
            Version = jobRunDto.Version
        };
        
        var updateJobRunResponse = await HttpClientHelper.PatchJobRun(_client, updateJobRunRequest);
        updateJobRunResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync(Constant.FinishedJobRunsEndpoint);
        response.EnsureSuccessStatusCode();

        var statisticsDto = await response.Content.ReadFromJsonAsync<FinishedJobRunsStatisticsDto>();
        Assert.NotNull(statisticsDto);
        Assert.NotEmpty(statisticsDto.JobRuns);

        var jobRunCount = statisticsDto.JobRuns.FirstOrDefault(x => x.JobId == createdJob.Id);
        Assert.NotNull(jobRunCount);
        Assert.Equal(createdJob.Id, jobRunCount.JobId);
        Assert.Equal(createdJob.Name, jobRunCount.JobName);
        
        Assert.Equal(1, statisticsDto.JobRuns.First().TotalJobRunCount);
        Assert.Equal(1, statisticsDto.JobRuns.First().JobRunCountByStatus[nameof(JobRunStatus.Completed)]);
        Assert.Equal(100, statisticsDto.JobRuns.First().JobRunPercentageByStatus[nameof(JobRunStatus.Completed)]);
    }
}