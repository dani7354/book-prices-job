using BookPricesJob.Application.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.Test.Setup;

public class InMemoryDatabaseContext : DatabaseContextBase
{
    public InMemoryDatabaseContext() { }
    public InMemoryDatabaseContext(DbContextOptions<DatabaseContextBase> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("BookPricesJob");
    }
}
