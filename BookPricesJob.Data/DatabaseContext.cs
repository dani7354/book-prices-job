using Microsoft.EntityFrameworkCore;
using BookPricesJob.Data.Entity;

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
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        EnvironmentHelper.LoadEnvFile();
        optionsBuilder.UseMySql(
            EnvironmentHelper.GetConnectionString(),
            new MySqlServerVersion(new Version(8, 0, 29)), b => b.EnableRetryOnFailure());
    }
}
