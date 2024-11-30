using BookPricesJob.Application.DatabaseContext;
using BookPricesJob.Data;
using BookPricesJob.Data.DatabaseContext;
using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookPricesJob.Test.Setup;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DatabaseContextBase));
            services.RemoveAll(typeof(IdentityDatabaseContextMysql));
            services.RemoveAll(typeof(IdentityDatabaseContextBase));
            services.RemoveAll(typeof(DbContextOptions<DatabaseContextBase>));
            services.RemoveAll(typeof(DbContextOptions<IdentityDatabaseContextBase>));
            services.RemoveAll(typeof(DbContextOptions<IdentityDatabaseContextMysql>));

            services.AddDbContext<DatabaseContextBase>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);

            services.AddDbContext<IdentityDatabaseContextBase>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);

            services.AddScoped<IPolicyEvaluator, FakePolicyEvaluator>();
            services.AddIdentityCore<ApiUser>()
                .AddEntityFrameworkStores<IdentityDatabaseContextBase>();
        });

        builder.UseEnvironment("Development");
    }
}
