using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;

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
}
