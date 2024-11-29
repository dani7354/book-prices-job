using System.Net;
using BookPricesJob.Test.Setup;

namespace BookPricesJob.Test.IntegrationTest;

public class TestBaseEndpoints(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

    [Theory]
    [InlineData("/api/jobs")]
    [InlineData("/api/jobruns")]
    public async Task Get_BaseEnpointsReturnCorrectStatusCodeAndContentType(string url)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
