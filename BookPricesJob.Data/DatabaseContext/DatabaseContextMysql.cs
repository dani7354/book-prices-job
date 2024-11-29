using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.DatabaseContext;

namespace BookPricesJob.Data;

public class DatabaseContextMysql : DatabaseContextBase
{
    public DatabaseContextMysql() { }
    public DatabaseContextMysql(DbContextOptions<DatabaseContextBase> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        EnvironmentHelper.LoadEnvFile();
        optionsBuilder.UseMySql(
            EnvironmentHelper.GetConnectionString(),
            new MySqlServerVersion(new Version(8, 4, 00)), b => b.EnableRetryOnFailure());
    }
}
