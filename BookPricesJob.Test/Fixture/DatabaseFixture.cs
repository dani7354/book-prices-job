using BookPricesJob.Application.DatabaseContext;
using BookPricesJob.Data.DatabaseContext;
using BookPricesJob.Data.Entity;
using BookPricesJob.Test.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BookPricesJob.Test.Fixture;

public class DatabaseFixture : IDisposable
{

    private bool _disposed = false;
    private readonly CustomWebApplicationFactory<Startup> _factory;
    public DatabaseFixture(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                var databaseContext = _factory.Services.GetService<DatabaseContextBase>();
                databaseContext?.Database.EnsureDeleted();

                var identityDatabaseContext = _factory.Services.GetService<IdentityDatabaseContextBase>();
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
