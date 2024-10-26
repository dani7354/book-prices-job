using BookPricesJob.Data.Entity;

namespace BookPricesJob.API.Service;

public interface ITokenService
{
    string CreateToken(ApiUser user);
}
