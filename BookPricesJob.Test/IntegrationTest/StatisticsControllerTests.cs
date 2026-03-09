using BookPricesJob.Test.Setup;

namespace BookPricesJob.Test.IntegrationTest;

public class StatisticsControllerTests
{
    private readonly HttpClient _client;

    public StatisticsControllerTests()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        var factory = new CustomWebApplicationFactory<Startup>();
        _client = factory.CreateClient();
    }
}