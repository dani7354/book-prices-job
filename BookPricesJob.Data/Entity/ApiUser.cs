using Microsoft.AspNetCore.Identity;

namespace BookPricesJob.Data.Entity;

public class ApiUser : IdentityUser
{
    public ICollection<ApiUserClaim> UserClaims { get; set; } = new List<ApiUserClaim>();
}
