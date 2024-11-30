using BookPricesJob.Data.DatabaseContext;
using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Data;

public class IdentityDatabaseContextMysql : IdentityDatabaseContextBase
{
    public IdentityDatabaseContextMysql() { }

    public IdentityDatabaseContextMysql(DbContextOptions<IdentityDatabaseContextBase> options) : base(options)
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
