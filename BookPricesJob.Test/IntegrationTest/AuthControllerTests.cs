using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;
using BookPricesJob.API.Model;
using System.Text.Json;
using System.Text;
using System.Net;

namespace BookPricesJob.Test.IntegrationTest;

public class AuthControllerTests : DatabaseFixture, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    public AuthControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsSuccess()
    {
        var password = "JensGodePassword123.";
        var UserRegisterRequest = new UserRegisterRequest()
        {
            UserName = "Jens",
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = new StringContent(
            JsonSerializer.Serialize(UserRegisterRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await _client.PostAsync($"{Constant.AuthBaseEndpoint}/register", registerPayload);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ValidUser_ReturnsSuccess()
    {
        var password = "SvendsGodePassword123.";
        var UserRegisterRequest = new UserRegisterRequest()
        {
            UserName = "Svend",
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = new StringContent(
            JsonSerializer.Serialize(UserRegisterRequest),
            Encoding.UTF8,
            "application/json");

        var registerResponse = await _client.PostAsync($"{Constant.AuthBaseEndpoint}/register", registerPayload);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginPayload = new StringContent(
            JsonSerializer.Serialize(new LoginRequest()
            {
                UserName = "Svend",
                Password = password
            }),
            Encoding.UTF8,
            "application/json");

        var loginResponse = await _client.PostAsync($"{Constant.AuthBaseEndpoint}/login", loginPayload);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }
}
