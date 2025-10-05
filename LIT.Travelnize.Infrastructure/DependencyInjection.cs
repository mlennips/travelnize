using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Infrastructure.Identity;
using LIT.Travelnize.Infrastructure.Messaging;
using LIT.Travelnize.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace LIT.Travelnize.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddMediatR();
            services.AddDatabase(configuration);
            services.AddIdentity(configuration);
            services.AddAudtiLog();
            services.AddRepository();
            services.AddUnitOfWork();
            services.AddSwaggerGen();
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = GetDatabaseConnectionString(configuration);

            if (CheckIsDev())
            {
                connectionString += ";Include Error Detail=true;";
            }

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<DbContextInitialiser>();

            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = GetIdentityConnectionString(configuration);

            services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ICurrentUser, CurrentUser>();

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

        private static IServiceCollection AddAudtiLog(this IServiceCollection services)
        {
            services.AddScoped<AuditSaveChangesInterceptor>();
            return services;
        }
        private static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IReadOnlyRepository<>), typeof(EFReadOnlyRepository<>));
        }

        private static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddMediatR(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.ManifestModule.Name?.StartsWith("LIT.", StringComparison.OrdinalIgnoreCase) ?? false).ToArray();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
            });
        }

        private static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        }

        private static string GetDatabaseConnectionString(IConfiguration configuration)
        {
            string? connectionString;

            if (CheckIsDesignTime()) // workaround for EF migrations
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

            if (CheckIsDesignTime()) // workaround for EF migrations
            {
                connectionString = "Host=localhost;Port=54944;Username=postgres;Password=j!(wQ72kUy8*6Cewge+arM;Database=identity";
            }
            else
            {
                connectionString = configuration.GetConnectionString("identity");
            }
            return connectionString ?? throw new InvalidOperationException("Connection string 'travelnize' not found.");
        }



        private static bool CheckIsDev()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDev = string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);
            return isDev;
        }

        private static bool CheckIsDesignTime()
        {
            return EF.IsDesignTime;
        }
    }
}
