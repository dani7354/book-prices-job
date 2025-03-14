using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Data.DatabaseContext;

public class DefaultDatabaseContext : DbContext
{
    public virtual DbSet<Job> Job { get; set; } = null!;
    public virtual DbSet<JobRun> JobRun { get; set; } = null!;
    public virtual DbSet<JobRunArgument> JobRunArgument { get; set; } = null!;
    public virtual DbSet<JobRunArgumentValue> JobRunArgumentValue { get; set; } = null!;
    public virtual DbSet<ApiUser> ApiUser { get; set; } = null!;
    public virtual DbSet<IdentityUserClaim<string>> ApiUserClaim { get; set; } = null!;

    public DefaultDatabaseContext() { }
    public DefaultDatabaseContext(DbContextOptions<DefaultDatabaseContext> options) : base(options) { }
}
