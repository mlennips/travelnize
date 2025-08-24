using LIT.Travelnize.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIT.Travelnize.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDatabase(configuration);
            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("travelnize");

            services.AddDbContext<TripDbContext>(options =>
                options.UseNpgsql(connectionString ?? throw new InvalidOperationException("Connection string 'travelnize' not found.")));

            return services;
        }
    }
}
