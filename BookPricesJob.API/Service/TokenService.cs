using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookPricesJob.Data.Entity;
using Microsoft.IdentityModel.Tokens;

namespace BookPricesJob.API.Service;

public class TokenService(string sigingKey) : ITokenService
{
    private const int TokenExpirationDays = 7;
    private readonly string _signingKey = sigingKey;

    public string CreateToken(ApiUser user)
    {
        var userId = user.Id;
        var userName = user.UserName!;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenExpirationDays),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
