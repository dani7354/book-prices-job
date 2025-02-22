using System.Net;
using System.Net.Http.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;

namespace BookPricesJob.Test.IntegrationTest;

public class JobControllerTests : DatabaseFixture, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public JobControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _client = factory.CreateClient();
    }

    public static IEnumerable<object[]> PartialUpdateRequests =>
        new List<object[]>
        {
            new object[]
            {
                new UpdateJobPartialRequest()
                {
                    Name = "Test Job 1 Updated",
                    IsActive = false
                }
            },
            new object[]
            {
                new UpdateJobPartialRequest()
                {
                    Name = "Test Job 1 Updated",
                }
            },
            new object[]
            {
                new UpdateJobPartialRequest()
                {
                    IsActive = false
                }
            }
        };

    [Fact]
    public async Task JobRuns_NoJobsOrJobRuns_ReturnsSuccessEmptyArray()
    {
        var response = await _client.GetAsync(Constant.JobsBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobRunDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Empty(jobRuns);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }

    [Fact]
    public async Task Create_NewJob_ReturnsSuccessAndCreatedObject()
    {
        var jobPayload = new CreateJobRequest()
        {
            Name = "Test Job 1",
            Description = "Test Job 1 described here",
            IsActive = true
        };

        var content = HttpClientHelper.CreateStringPayload(jobPayload);

        var responseCreateJob = await _client.PostAsync(Constant.JobsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.Created, responseCreateJob.StatusCode);
        Assert.Equal(
            "application/json; charset=utf-8",
            responseCreateJob.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Create_InvalidJob_ReturnsBadRequest()
    {
        var jobRunPayload = new CreateJobRequest()
        {
            Name = "Test Job 1",
            IsActive = true
        };

        var content = HttpClientHelper.CreateStringPayload(jobRunPayload);

        var responseCreateJob = await _client.PostAsync(Constant.JobsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.BadRequest, responseCreateJob.StatusCode);
    }

    [Fact]
    public async Task UpdateFull_ExistingJob_ReturnsSuccess()
    {
        var jobPayload = TestData.GetCreateJobRequest();
        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
        var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobUpdatePayload = new UpdateJobFullRequest()
        {
            Id = job!.Id,
            Name = "Test Job 1 Updated",
            Description = "Test Job 1 described here Updated",
            Version = job.Version,
            IsActive = false
        };

        var content = HttpClientHelper.CreateStringPayload(jobUpdatePayload);
        var responseUpdateJob = await _client.PutAsync($"{Constant.JobsBaseEndpoint}/{job.Id}", content);

        Assert.Equal(HttpStatusCode.OK, responseUpdateJob.StatusCode);
    }

    [Fact]
    public async Task UpdateFull_InvalidJobId_ReturnsBadRequest()
    {
        var jobPayload = TestData.GetCreateJobRequest();
        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
        var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobUpdatePayload = new UpdateJobFullRequest()
        {
            Id = Guid.NewGuid().ToString(),
            Name = job!.Name,
            Description = job.Description,
            Version = job.Version,
            IsActive = false
        };

        var content = HttpClientHelper.CreateStringPayload(jobUpdatePayload);

        var responseUpdateJob = await _client.PutAsync($"{Constant.JobsBaseEndpoint}/{jobUpdatePayload.Id}", content);

        Assert.Equal(HttpStatusCode.NotFound, responseUpdateJob.StatusCode);
    }

    [Theory]
    [MemberData(nameof(PartialUpdateRequests))]
    public async Task UpdatePartial_ExistingJob_ReturnsSuccess(UpdateJobPartialRequest jobUpdatePayload)
    {
        var jobPayload = TestData.GetCreateJobRequest();
        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
        var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        jobUpdatePayload = TestData.GetUpdatePartialRequest(
            job!.Id, 
            job.Version, 
            name: jobUpdatePayload.Name,
            description: jobUpdatePayload.Description,
            isActive: jobUpdatePayload.IsActive);
        
        var content = HttpClientHelper.CreateStringPayload(jobUpdatePayload);

        var responseUpdateJob = await _client.PatchAsync($"{Constant.JobsBaseEndpoint}/{job!.Id}", content);

        Assert.Equal(HttpStatusCode.OK, responseUpdateJob.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingJob_ReturnsSuccess()
    {
        var jobPayload = TestData.GetCreateJobRequest();
        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
        var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var responseDeleteJob = await _client.DeleteAsync($"{Constant.JobsBaseEndpoint}/{job!.Id}");

        Assert.Equal(HttpStatusCode.OK, responseDeleteJob.StatusCode);
    }

    [Fact]
    public async Task Delete_NoJobs_ReturnsNotFound()
    {
        var responseDeleteJob = await _client.DeleteAsync($"{Constant.JobsBaseEndpoint}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, responseDeleteJob.StatusCode);
    }
}
