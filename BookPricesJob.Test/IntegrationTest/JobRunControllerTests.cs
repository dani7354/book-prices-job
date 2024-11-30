using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;
using BookPricesJob.Common.Domain;

namespace BookPricesJob.Test.IntegrationTest;

public class JobRunControllerTests : DatabaseFixture, IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public JobRunControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _client = factory.CreateClient();
    }

    public static IEnumerable<object[]> JobRunNewStatusesAndPriorities => new List<object[]>
    {
        new object[] { JobRunPriority.High, JobRunStatus.Running },
        new object[] { JobRunPriority.Low, JobRunStatus.Failed },
        new object[] { JobRunPriority.Normal, JobRunStatus.Completed },
    };

    private async Task<JobRunDto> CreateJobWithJobRun(
        JobRunPriority priority = JobRunPriority.Normal)
    {
        var responseCreateJob = await HttpClientHelper.PostJob(_client, TestData.CreateJobRequestOne);
        var jobDto = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobId = jobDto!.Id;
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = priority.ToString()
        };

        var content = new StringContent(
            JsonSerializer.Serialize(jobRunPayload),
            Encoding.UTF8,
            "application/json");

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);
        var jobRunDto = await responseCreateJobRun.Content.ReadFromJsonAsync<JobRunDto>();
        Assert.NotNull(jobRunDto);

        return jobRunDto;
    }

    [Fact]
    public async Task JobRuns_NoJobsOrJobRuns_ReturnsSuccessEmptyArray()
    {
        var response = await _client.GetAsync(Constant.JobRunsBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Empty(jobRuns);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }

    [Fact]
    public async Task Create_NewJobRun_ReturnsSuccessAndCreatedObject()
    {
        var responseCreateJob = await HttpClientHelper.PostJob(_client, TestData.CreateJobRequestOne);
        var jobDto = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobId = jobDto!.Id;
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = JobRunPriority.Normal.ToString()
        };

        var content = new StringContent(
            JsonSerializer.Serialize(jobRunPayload),
            Encoding.UTF8,
            "application/json");

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.Created, responseCreateJobRun.StatusCode);
        Assert.Equal(
            "application/json; charset=utf-8",
            responseCreateJobRun.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Create_NoJobsExist_ReturnsBadRequest()
    {
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = Guid.NewGuid().ToString(),
            Priority = "Normal"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(jobRunPayload),
            Encoding.UTF8,
            "application/json");

        var responseCreateJobRun = await _client.PostAsync(Constant.JobRunsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.BadRequest, responseCreateJobRun.StatusCode);
    }

    [Theory]
    [MemberData(nameof(JobRunNewStatusesAndPriorities))]
    public async Task UpdateFull_JobRun_ReturnsSuccessAndUpdatedStatus(JobRunPriority newPriority, JobRunStatus newStatus)
    {
        var jobRunDto = await this.CreateJobWithJobRun(JobRunPriority.Normal);

        var updateJobRunPayload = new UpdateJobRunFullRequest()
        {
            JobRunId = jobRunDto!.Id,
            JobId = jobRunDto.JobId,
            Priority = newPriority.ToString(),
            Status = newStatus.ToString()
        };

        var updateContent = new StringContent(
            JsonSerializer.Serialize(updateJobRunPayload),
            Encoding.UTF8,
            "application/json");

        var responseUpdateJobRun = await _client.PutAsync(
            $"{Constant.JobRunsBaseEndpoint}/{jobRunDto!.Id}",
            updateContent);

        var reponseGetJobRunUpdated = await _client.GetAsync($"{Constant.JobRunsBaseEndpoint}/{jobRunDto!.Id}");
        var jobRunUpdated = await reponseGetJobRunUpdated.Content.ReadFromJsonAsync<JobRunDto>();

        Assert.Equal(HttpStatusCode.OK, responseUpdateJobRun.StatusCode);
        Assert.NotNull(jobRunUpdated);
        Assert.Equal(newStatus.ToString(), jobRunUpdated.Status);
        Assert.Equal(newPriority.ToString(), jobRunUpdated.Priority);
    }
}
