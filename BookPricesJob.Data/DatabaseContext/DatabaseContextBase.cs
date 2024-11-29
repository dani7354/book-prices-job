using BookPricesJob.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Application.DatabaseContext;

public class DatabaseContextBase : DbContext
{
    public virtual DbSet<Job> Job { get; set; } = null!;
    public virtual DbSet<JobRun> JobRun { get; set; } = null!;
    public virtual DbSet<JobRunArgument> JobRunArgument { get; set; } = null!;
    public virtual DbSet<JobRunArgumentValue> JobRunArgumentValue { get; set; } = null!;

    public DatabaseContextBase() { }
    public DatabaseContextBase(DbContextOptions<DatabaseContextBase> options) : base(options) { }
}
