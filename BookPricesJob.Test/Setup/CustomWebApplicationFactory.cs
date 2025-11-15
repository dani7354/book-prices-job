using BookPricesJob.Application.Contract;
using BookPricesJob.Data.DatabaseContext;
using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookPricesJob.Test.Setup;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DefaultDatabaseContext));
            services.RemoveAll(typeof(DbContextOptions<DefaultDatabaseContext>));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<DefaultDatabaseContext>));
            
            services.AddDbContext<DefaultDatabaseContext>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.RemoveAll(typeof(ICache));
            services.AddScoped<ICache, FakeCache>();

            services.AddScoped<IPolicyEvaluator, FakePolicyEvaluator>();
            services.AddIdentityCore<ApiUser>()
                .AddEntityFrameworkStores<DefaultDatabaseContext>();
        });

        builder.UseEnvironment("Development");
    }
}
