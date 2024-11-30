using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;


namespace BookPricesJob.Test.Setup;

public class FakePolicyEvaluator : IPolicyEvaluator
{
    public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        var principal = new ClaimsPrincipal();
        principal.AddIdentity(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, API.Constant.JobManagerClaim),
                    new Claim(ClaimTypes.Role, API.Constant.JobRunnerClaim),
                ], "FakeScheme"));

        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(principal,new AuthenticationProperties(), "FakeScheme")));
    }

    public Task<PolicyAuthorizationResult> AuthorizeAsync(
        AuthorizationPolicy policy,
        AuthenticateResult authenticationResult,
        HttpContext context,
        object? resource)
    {
        return Task.FromResult(PolicyAuthorizationResult.Success());
    }
}
