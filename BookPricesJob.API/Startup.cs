using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Application.Service;
using BookPricesJob.Data.Repository;
using BookPricesJob.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookPricesJob.API.Model;
using BookPricesJob.Common.Domain;


namespace BookPricesJob.API;
public class Startup
{
    public Startup(IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            EnvironmentHelper.LoadEnvFile();

        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile(env.ContentRootPath + "/appsettings.json")
            .AddJsonFile(env.ContentRootPath + "/appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddResponseCaching();

        var mysqlServerVersion = new MySqlServerVersion(new Version(8, 0, 29));
        services.AddDbContext<DatabaseContext>(
            options => options.UseMySql(
                EnvironmentHelper.GetConnectionString(),
                mysqlServerVersion, b => b.EnableRetryOnFailure()));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetValue<string>(Constant.JwtIssuer),
                    ValidAudience = Configuration.GetValue<string>(Constant.JwtAudience),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration.GetValue<string>(Constant.JwtSigningKey) ?? ""))
                };
            });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJobService, JobService>();

        var dtoMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateJobDto, Job>();
            cfg.CreateMap<Job, JobListItemDto>();
        }).CreateMapper();
        services.AddSingleton(dtoMapper);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseResponseCaching();
    }
}
