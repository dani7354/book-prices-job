using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookPricesJob.Data.Entity;
using Microsoft.IdentityModel.Tokens;

namespace BookPricesJob.API.Service;

public class TokenService : ITokenService
{
    private const int TokenExpirationDays = 7;
    private readonly string _tokenSigningKey;
    private readonly string _tokenAudience;
    private readonly string _tokenIssuer;

    public TokenService(string tokenSigningKey, string tokenAudience, string tokenIssuer)
    {
        _tokenSigningKey = tokenSigningKey;
        _tokenAudience = tokenAudience;
        _tokenIssuer = tokenIssuer;
    }

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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _tokenIssuer,
            Audience = _tokenAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(TokenExpirationDays),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
