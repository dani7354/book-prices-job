using BookPricesJob.Test.Fixture;
using BookPricesJob.Test.Setup;
using BookPricesJob.API.Model;
using System.Net;

namespace BookPricesJob.Test.IntegrationTest;

public class AuthControllerTests : DatabaseFixture, IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private const string RegisterEndpoint = $"{Constant.AuthBaseEndpoint}/register";
    private const string LoginEndpoint = $"{Constant.AuthBaseEndpoint}/login";
    
    private readonly HttpClient _client;
    public AuthControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _client = factory.CreateClient();
    }

    private static HttpClient CreateClientNewUsersAllowed()
    {
        Environment.SetEnvironmentVariable(API.Constant.AllowNewUsers, "true");
        return new CustomWebApplicationFactory<Startup>().CreateClient();
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsSuccess()
    {
        var client = CreateClientNewUsersAllowed();
        var password = "Jens'GodePassword123.";
        var userRegisterRequest = new UserRegisterRequest()
        {
            UserName = "Jens",
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = HttpClientHelper.CreateStringPayload(userRegisterRequest);

        var registerResponse = await client.PostAsync(RegisterEndpoint, registerPayload);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
    }

    [Fact]
    public async Task Register_NewUsersDeactivated_ReturnsBadRequest()
    {
        var password = "Jens'GodePassword123.";
        var userRegisterRequest = new UserRegisterRequest()
        {
            UserName = "Jens",
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = HttpClientHelper.CreateStringPayload(userRegisterRequest);

        var registerResponse = await _client.PostAsync(RegisterEndpoint, registerPayload);

        Assert.Equal(HttpStatusCode.BadRequest, registerResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ValidUserAndNewUsersAllowed_ReturnsSuccess()
    {
        var password = "SvendsGodePassword123.";
        var userRegisterRequest = new UserRegisterRequest()
        {
            UserName = "Svend",
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = HttpClientHelper.CreateStringPayload(userRegisterRequest);

        var client = CreateClientNewUsersAllowed();
        var registerResponse = await client.PostAsync(RegisterEndpoint, registerPayload);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginRequest = new LoginRequest()
        {
            UserName = "Svend",
            Password = password
        };
        var loginPayload = HttpClientHelper.CreateStringPayload(loginRequest);

        var loginResponse = await client.PostAsync(LoginEndpoint, loginPayload);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }
}
