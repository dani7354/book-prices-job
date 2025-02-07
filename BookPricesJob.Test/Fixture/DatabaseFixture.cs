using BookPricesJob.Application.DatabaseContext;
using BookPricesJob.Data.DatabaseContext;
using BookPricesJob.Test.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace BookPricesJob.Test.Fixture;

public class DatabaseFixture(CustomWebApplicationFactory<Startup> factory) : IDisposable
{

    private bool _disposed;

    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                var databaseContext = factory.Services.GetService<DatabaseContextBase>();
                databaseContext?.Database.EnsureDeleted();

                var identityDatabaseContext = factory.Services.GetService<IdentityDatabaseContextBase>();
                identityDatabaseContext?.Database.EnsureDeleted();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
