using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BookPricesJob.API.Model;
using BookPricesJob.Data.Entity;
using BookPricesJob.API.Service;

namespace BookPricesJob.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<ApiUser> _userManager;
    private readonly SignInManager<ApiUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager,
        ITokenService tokenService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequest)
    {
        _logger.LogInformation("Logging in user {0}...", loginRequest.UserName);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = loginRequest.UserName;
        var password = loginRequest.Password;
        var result = await _signInManager.PasswordSignInAsync(
            userName: userName,
            password: password,
            isPersistent: false,
            lockoutOnFailure:  false);


        if (result.Succeeded)
        {
            _logger.LogInformation("User {0} logged in successfully!", userName);
            var user = await _userManager.FindByNameAsync(userName);

            return Ok(_tokenService.CreateToken(user!));
        }

        return Unauthorized();
    }


    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerRequest)
    {
        _logger.LogInformation("Registering user {0}...", registerRequest.UserName);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = Guid.NewGuid().ToString();

        var user = new ApiUser
        {
            Id = userId,
            UserName = registerRequest.UserName,
            UserClaims = new List<ApiUserClaim>
            {
                new ApiUserClaim
                {
                    UserId = userId,
                    ClaimType = "role",
                    ClaimValue = Constant.JobManagerClaim
                }
            }
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User {0} registered successfully!", registerRequest.UserName);
            return Ok();
        }

        return BadRequest(result.Errors);
    }
}
