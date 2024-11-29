using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Data;

public class IdentityDatabaseContext : IdentityDbContext<ApiUser>
{

    public DbSet<ApiUser> ApiUser { get; set; } = null!;
    public DbSet<ApiUserClaim> ApiUserClaim { get; set; } = null!;

    public IdentityDatabaseContext() { }

    public IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        EnvironmentHelper.LoadEnvFile();
        optionsBuilder.UseMySql(
            EnvironmentHelper.GetConnectionString(),
            new MySqlServerVersion(new Version(8, 4, 00)), b => b.EnableRetryOnFailure());
    }
}
