using LIT.Travelnize.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LIT.Travelnize.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDatabase(configuration);
            services.AddIdentity(configuration);
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetDatabaseConnectionString(configuration);
            
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContextInitialiser>();

            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = GetIdentityConnectionString(configuration);

            services.AddDbContext<AppIdentityDbContext>(options => options.UseNpgsql(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

            return services;
        }

        private static string GetDatabaseConnectionString(IConfiguration configuration)
        {
            string? connectionString;

            if (EF.IsDesignTime) // workaround for EF migrations
            {
                connectionString = "Host=localhost;Port=54944;Username=postgres;Password=j!(wQ72kUy8*6Cewge+arM;Database=travelnize";
            }
            else
            {
                connectionString = configuration.GetConnectionString("travelnize");
            }
            return connectionString ?? throw new InvalidOperationException("Connection string 'travelnize' not found.");
        }

        private static string GetIdentityConnectionString(IConfiguration configuration)
        {
            string? connectionString;

            if (EF.IsDesignTime) // workaround for EF migrations
            {
                connectionString = "Host=localhost;Port=54944;Username=postgres;Password=j!(wQ72kUy8*6Cewge+arM;Database=identity";
            }
            else
            {
                connectionString = configuration.GetConnectionString("identity");
            }
            return connectionString ?? throw new InvalidOperationException("Connection string 'travelnize' not found.");
        }
    }
}
