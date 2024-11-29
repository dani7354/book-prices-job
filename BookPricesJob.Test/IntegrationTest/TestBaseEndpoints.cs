using System.Net;
using BookPricesJob.Test.Setup;
using BookPricesJob.API.Model;
using System.Text.Json;
using System.Text;

namespace BookPricesJob.Test.IntegrationTest;

public class TestBaseEndpoints(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private const string JobsBaseEndpoint = "/api/jobs";
    private const string JobRunsBaseEndpoint = "/api/jobruns";
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

    [Theory]
    [InlineData(JobsBaseEndpoint)]
    [InlineData(JobRunsBaseEndpoint)]
    public async Task Get_BaseEnpointsReturnCorrectStatusCodeAndContentType(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Post_JobsEndpointReturnsSuccessAndContentType()
    {
        var client = _factory.CreateClient();

        var jobPayload = new CreateJobRequest()
        {
            Name = "Test Job 1",
            Description = "Test Job 1 described here",
            IsActive = true
        };

        var content = new StringContent(JsonSerializer.Serialize(jobPayload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(JobsBaseEndpoint, content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
