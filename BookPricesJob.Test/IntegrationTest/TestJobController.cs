
using BookPricesJob.API;
using BookPricesJob.Data;
using BookPricesJob.Test.Setup;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookPricesJob.Test.IntegrationTest;
public class TestJobController : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestJobController(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private static void SetNeededEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable(Data.Constant.JwtIssuer, "localhost");
        Environment.SetEnvironmentVariable(Data.Constant.JwtAudience, "localhost");
        Environment.SetEnvironmentVariable(Data.Constant.JwtSigningKey, "THISISASECRETKEY");
    }

    [Fact]
    public void GetAllJobs()
    {
        SetNeededEnvironmentVariables();
        var client = _factory.CreateClient();

        var response = client.GetAsync("/api/jobs").Result;
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            Assert.True(response.IsSuccessStatusCode);
    }
}
