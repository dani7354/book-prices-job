using BookPricesJob.Application.DatabaseContext;
using BookPricesJob.Data;
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
            services.RemoveAll(typeof(IdentityDatabaseContext));
            services.RemoveAll(typeof(DbContextOptions<DatabaseContextBase>));
            services.RemoveAll(typeof(DbContextOptions<IdentityDatabaseContext>));

            services.AddDbContext<DatabaseContextBase>(options => options.UseInMemoryDatabase("BookPricesJob"));
            services.AddDbContext<IdentityDatabaseContext>(options => options.UseInMemoryDatabase("BookPricesJobAuth"));

            services.AddScoped<IPolicyEvaluator, FakePolicyEvaluator>();
        });


        builder.UseEnvironment("Development");
    }
}
