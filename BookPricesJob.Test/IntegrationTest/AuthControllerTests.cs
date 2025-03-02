using BookPricesJob.Test.Setup;
using BookPricesJob.API.Model;
using System.Net;
using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace BookPricesJob.Test.IntegrationTest;

// This test class tests the AuthController endpoints (Login, Register etc.), not the authentication and authorization
// of the web API.
public class AuthControllerTests
{
    private const string RegisterEndpoint = $"{Constant.AuthBaseEndpoint}/register";
    private const string LoginEndpoint = $"{Constant.AuthBaseEndpoint}/login";
    private const string AddRoleEndpoint = $"{Constant.AuthBaseEndpoint}/addrole";
    private const string RemoveRoleEndpoint = $"{Constant.AuthBaseEndpoint}/removerole";
    
    private readonly CustomWebApplicationFactory<Startup> _factory;
    
    public AuthControllerTests()
    {
        EnvironmentHelper.SetNecessaryEnvironmentVariables();
        _factory = new CustomWebApplicationFactory<Startup>();
    }

    private HttpClient CreateClientNewUsersAllowed()
    {
        Environment.SetEnvironmentVariable(API.Constant.AllowNewUsers, true.ToString());
        return _factory.CreateClient();
    }

    private HttpClient CreateClientNewUsersNotAllowed()
    {
        Environment.SetEnvironmentVariable(API.Constant.AllowNewUsers, false.ToString());
        return _factory.CreateClient();
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
        var client = CreateClientNewUsersNotAllowed();
        var password = "Jens'GodePassword123.";
        var userRegisterRequest = new UserRegisterRequest()
        {
            UserName = "Jens",
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = HttpClientHelper.CreateStringPayload(userRegisterRequest);
        var registerResponse = await client.PostAsync(RegisterEndpoint, registerPayload);

        Assert.Equal(HttpStatusCode.BadRequest, registerResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ValidUserAndNewUsersAllowed_ReturnsSuccess()
    {
        var client = CreateClientNewUsersAllowed();
        var (username, password) = await RegisterTestUser(client);

        var loginRequest = new LoginRequest()
        {
            UserName = username,
            Password = password
        };
        
        var loginPayload = HttpClientHelper.CreateStringPayload(loginRequest);
        var loginResponse = await client.PostAsync(LoginEndpoint, loginPayload);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task AddRole_ValidRole_ReturnsSuccessRoleAdded()
    {
        var client = CreateClientNewUsersAllowed();
        using var scope = _factory.Services.CreateScope();
        
        var (username, _) = await RegisterTestUser(client);

        var addedRole = API.Constant.JobManagerClaim;
        await AddRoleToUser(client, username, addedRole);

        var userManager = scope.ServiceProvider.GetService<UserManager<ApiUser>>();
        Assert.NotNull(userManager);
        
        var user = await userManager.FindByNameAsync(username);
        Assert.NotNull(user);
        
        var userClaims = await userManager.GetClaimsAsync(user);
        Assert.Contains(
            userClaims, 
            claim => claim is { Type: API.Constant.ClaimRoleType, Value: API.Constant.JobManagerClaim });
    }
    
    [Fact]
    public async Task RemoveRole_ValidRole_ReturnsSuccessRoleRemoved()
    {
        var client = CreateClientNewUsersAllowed();
        using var scope = _factory.Services.CreateScope();
        
        var (username, _) = await RegisterTestUser(client);

        var addedRole = API.Constant.JobRunnerClaim;
        await AddRoleToUser(client, username, addedRole);

        var removeRoleRequest = new RemoveRoleRequest
        {
            UserName = username,
            RoleName = addedRole
        };
        
        var removeRolePayload = HttpClientHelper.CreateStringPayload(removeRoleRequest);
        var removeRoleResponse = await client.PostAsync(RemoveRoleEndpoint, removeRolePayload);
        Assert.Equal(HttpStatusCode.OK, removeRoleResponse.StatusCode);

        var userManager = scope.ServiceProvider.GetService<UserManager<ApiUser>>();
        Assert.NotNull(userManager);
        
        var user = await userManager.FindByNameAsync(username);
        Assert.NotNull(user);
        
        var userClaims = await userManager.GetClaimsAsync(user);
        Assert.Empty(userClaims);
    }

    private static async Task AddRoleToUser(HttpClient client, string username, string roleName)
    {
        var addRoleRequest = new AddRoleRequest
        {
            UserName = username,
            RoleName = roleName
        };
        var addRolePayload = HttpClientHelper.CreateStringPayload(addRoleRequest);
        var addRoleResponse = await client.PostAsync(AddRoleEndpoint, addRolePayload);
        
        Assert.Equal(HttpStatusCode.OK, addRoleResponse.StatusCode);
    }

    private static async Task<(string user, string password)> RegisterTestUser(
        HttpClient client,
        string username = "Svend",
        string password = "SvendsGodePassword123'")
    {
        var userRegisterRequest = new UserRegisterRequest()
        {
            UserName = username,
            Password = password,
            ConfirmPassword = password
        };

        var registerPayload = HttpClientHelper.CreateStringPayload(userRegisterRequest);
        var registerResponse = await client.PostAsync(RegisterEndpoint, registerPayload);
        
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        return (username, password);
    }
}
