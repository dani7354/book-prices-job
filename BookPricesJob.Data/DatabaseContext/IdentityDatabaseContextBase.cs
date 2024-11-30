using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Data.DatabaseContext;

public class IdentityDatabaseContextBase : IdentityDbContext<ApiUser>
{
    public virtual DbSet<ApiUser> ApiUser { get; set; } = null!;
    public virtual DbSet<ApiUserClaim> ApiUserClaim { get; set; } = null!;

    public IdentityDatabaseContextBase() { }
    public IdentityDatabaseContextBase(DbContextOptions<IdentityDatabaseContextBase> options) : base(options)
    {
        Database.EnsureCreated();
    }

}
