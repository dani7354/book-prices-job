using System.Net;
using System.Net.Http.Json;
using BookPricesJob.API.Model;
using BookPricesJob.Test.Setup;

namespace BookPricesJob.Test.IntegrationTest;

public class JobControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public JobControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Jobs_NoJobs_ReturnsSuccessEmptyArray()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Constant.JobsBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var jobRuns = await response.Content.ReadFromJsonAsync<JobDto[]>();
        Assert.NotNull(jobRuns);
        Assert.Empty(jobRuns);

        var contentType = response.Content.Headers.ContentType?.ToString();
        Assert.Equal("application/json; charset=utf-8", contentType);
    }

    [Fact]
    public async Task Post_JobsEndpointReturnsSuccessAndContentType()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();

        var client = _factory.CreateClient();

        var response = await HttpClientHelper.PostJob(client, TestData.CreateJobRequestOne);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
