
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

    [Fact]
    public void GetAllJobs()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        var client = _factory.CreateClient();

        var response = client.GetAsync("/api/jobs").Result;
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            Assert.True(response.IsSuccessStatusCode);
    }
}
