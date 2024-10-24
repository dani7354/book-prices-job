using Microsoft.AspNetCore.Mvc;
using BookPricesJob.Application.Contract;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using BookPricesJob.API.Model;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(ILogger<AuthController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var email = request.Email!;
        var password = request.Password!;
        var result = await _signInManager.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure:  false);
        if (result.Succeeded)
            return Ok();

        return Unauthorized();

    }
}
