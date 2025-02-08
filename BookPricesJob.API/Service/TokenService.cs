using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookPricesJob.Data.Entity;
using Microsoft.IdentityModel.Tokens;

namespace BookPricesJob.API.Service;

public class TokenService(string tokenSigningKey, string tokenAudience, string tokenIssuer) : ITokenService
{
    private const int TokenExpirationDays = 7;

    public string CreateToken(ApiUser user, IEnumerable<Claim> userClaims)
    {
        var userId = user.Id;
        var userName = user.UserName!;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId),
            new(JwtRegisteredClaimNames.UniqueName, userName)
        };

        claims.AddRange(userClaims.Where(uc => uc.Type != null && uc.Value != null));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = tokenIssuer,
            Audience = tokenAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenExpirationDays),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
