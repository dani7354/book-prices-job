using Microsoft.EntityFrameworkCore;
using BookPricesJob.Application.Contract;
using BookPricesJob.Application.Service;
using BookPricesJob.Data.Repository;
using BookPricesJob.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using BookPricesJob.Data.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using BookPricesJob.API.Service;


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
        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
            options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
            options.RespectBrowserAcceptHeader = false;
        });


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookPricesJob.API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

        });

        services.AddResponseCaching();

        var mysqlServerVersion = new MySqlServerVersion(new Version(8, 4, 00));
        services.AddDbContext<DatabaseContext>(
            options => options.UseMySql(
                EnvironmentHelper.GetConnectionString(),
                mysqlServerVersion, b => b.EnableRetryOnFailure()));

          services.AddDbContext<IdentityDatabaseContext>(
            options => options.UseMySql(
                EnvironmentHelper.GetConnectionString(),
                mysqlServerVersion, b => b.EnableRetryOnFailure()));

        services.AddHttpContextAccessor();
        services.AddScoped<UserManager<ApiUser>>();
        services.AddScoped<SignInManager<ApiUser>>();
        services.AddIdentityCore<ApiUser>()
            .AddEntityFrameworkStores<IdentityDatabaseContext>();

        var jwtIssuer = Configuration.GetValue<string>(Data.Constant.JwtIssuer)??
            throw new Exception("JWT issuer is missing");
        var jwtAudience = Configuration.GetValue<string>(Data.Constant.JwtAudience) ??
            throw new Exception("JWT audience is missing");
        var jwtSigningKey = Configuration.GetValue<string>(Data.Constant.JwtSigningKey) ??
            throw new Exception("JWT signing key is missing");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddBearerToken()
        .AddCookie("Identity.Application")
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey))
            };
        });

        services.AddAuthorization(options => {
            options.AddPolicy(Constant.JobManagerPolicy, policy => policy.RequireRole(Constant.JobManagerClaim));
            options.AddPolicy(Constant.JobRunnerPolicy, policy => policy.RequireRole(Constant.JobRunnerClaim));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<ITokenService, TokenService>(
            x => new TokenService(jwtSigningKey, jwtAudience, jwtIssuer));
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
