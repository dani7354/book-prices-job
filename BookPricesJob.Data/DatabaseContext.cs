using Microsoft.EntityFrameworkCore;
using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BookPricesJob.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Job> Job { get; set; }
    public DbSet<JobRun> JobRun { get; set; }
    public DbSet<JobRunArgument> JobRunArgument { get; set; }
    public DbSet<JobRunArgumentValue> JobRunArgumentValue { get; set; }


    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        EnvironmentHelper.LoadEnvFile();
        optionsBuilder.UseMySql(
            EnvironmentHelper.GetConnectionString(),
            new MySqlServerVersion(new Version(8, 4, 00)), b => b.EnableRetryOnFailure());
    }
}
