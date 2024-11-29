using System.Net;
using BookPricesJob.Test.Setup;
using BookPricesJob.API.Model;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;

namespace BookPricesJob.Test.IntegrationTest;

public class TestBaseEndpoints(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string JobsBaseEndpoint = "/api/jobs";
    private const string JobRunsBaseEndpoint = "/api/jobruns";
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

    private static async Task<HttpResponseMessage> PostJob(HttpClient client, CreateJobRequest job)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(job),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(JobsBaseEndpoint, content);
        response.EnsureSuccessStatusCode();

        return response;
    }

    [Theory]
    [InlineData(JobsBaseEndpoint)]
    [InlineData(JobRunsBaseEndpoint)]
    public async Task Get_BaseEnpointsReturnCorrectStatusCodeAndContentType(string url)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();

        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Post_JobsEndpointReturnsSuccessAndContentType()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();

        var client = _factory.CreateClient();

        var response = await PostJob(client, TestData.CreateJobRequestOne);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Post_JobRunsEndpointReturnsSuccessAndContentType()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        var client = _factory.CreateClient();

        var responseCreateJob = await PostJob(client, TestData.CreateJobRequestOne);
        var jobDto = await responseCreateJob.Content.ReadFromJsonAsync<JobDto>();

        var jobId = jobDto!.Id;
        var jobRunPayload = new CreateJobRunRequest()
        {
            JobId = jobId,
            Priority = "Normal"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(jobRunPayload),
            Encoding.UTF8,
            "application/json");

        var responseCreateJobRun = await client.PostAsync(JobRunsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.Created, responseCreateJobRun.StatusCode);
        Assert.Equal(
            "application/json; charset=utf-8",
            responseCreateJobRun.Content.Headers.ContentType?.ToString());
    }
}
