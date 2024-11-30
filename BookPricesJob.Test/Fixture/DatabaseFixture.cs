using BookPricesJob.Application.DatabaseContext;
using BookPricesJob.Test.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace BookPricesJob.Test.Fixture;

public class DatabaseFixture : IDisposable
{

    private bool _disposed = false;
    private readonly CustomWebApplicationFactory<Program> _factory;
    public DatabaseFixture(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                var databaseContext = _factory.Services.GetRequiredService<DatabaseContextBase>();
                databaseContext.Database.EnsureDeleted();
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
