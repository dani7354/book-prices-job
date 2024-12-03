using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;
using Newtonsoft.Json.Linq;

namespace BookPricesJob.Test.IntegrationTest;

public class JobControllerTests : DatabaseFixture, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public JobControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _factory = factory;
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

        var content = new StringContent(
            JsonSerializer.Serialize(jobPayload),
            Encoding.UTF8,
            "application/json");

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

        var content = new StringContent(
            JsonSerializer.Serialize(jobRunPayload),
            Encoding.UTF8,
            "application/json");

        var responseCreateJob = await _client.PostAsync(Constant.JobsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.BadRequest, responseCreateJob.StatusCode);
    }

    [Fact]
    public async Task UpdateFull_ExistingJob_ReturnsSuccess()
    {
        var jobPayload = new CreateJobRequest()
        {
            Name = "Test Job 1",
            Description = "Test Job 1 described here",
            IsActive = true
        };

        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
        var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobUpdatePayload = new UpdateJobFullRequest()
        {
            Id = job!.Id,
            Name = "Test Job 1 Updated",
            Description = "Test Job 1 described here Updated",
            IsActive = false
        };

        var content = new StringContent(
            JsonSerializer.Serialize(jobUpdatePayload),
            Encoding.UTF8,
            "application/json");

        var responseUpdateJob = await _client.PutAsync($"{Constant.JobsBaseEndpoint}/{job.Id}", content);

        Assert.Equal(HttpStatusCode.OK, responseUpdateJob.StatusCode);
    }

    [Fact]
    public async Task UpdateFull_InvalidJobId_ReturnsBadRequest()
    {
         var jobPayload = new CreateJobRequest()
        {
            Name = "Test Job 1",
            Description = "Test Job 1 described here",
            IsActive = true
        };

        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
         var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobUpdatePayload = new UpdateJobFullRequest()
        {
            Id = Guid.NewGuid().ToString(),
            Name = job!.Name,
            Description = job.Description,
            IsActive = false
        };

        var content = new StringContent(
            JsonSerializer.Serialize(jobUpdatePayload),
            Encoding.UTF8,
            "application/json");

        var responseUpdateJob = await _client.PutAsync($"{Constant.JobsBaseEndpoint}/{jobUpdatePayload.Id}", content);

        Assert.Equal(HttpStatusCode.NotFound, responseUpdateJob.StatusCode);
    }

    [Theory]
    [MemberData(nameof(PartialUpdateRequests))]
    public async Task UpdatePartial_ExistingJob_ReturnsSuccess(UpdateJobPartialRequest jobUpdatePayload)
    {
        var jobPayload = new CreateJobRequest()
        {
            Name = "Test Job 1",
            Description = "Test Job 1 described here",
            IsActive = true
        };

        var responseCreateJob = await HttpClientHelper.PostJob(_client, jobPayload);
        var job = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        jobUpdatePayload.Id = job!.Id;
        var content = new StringContent(
            JsonSerializer.Serialize(jobUpdatePayload),
            Encoding.UTF8,
            "application/json");

        var responseUpdateJob = await _client.PatchAsync($"{Constant.JobsBaseEndpoint}/{job!.Id}", content);

        Assert.Equal(HttpStatusCode.OK, responseUpdateJob.StatusCode);
    }
}
