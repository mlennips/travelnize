using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Travelnize.Infrastructure
{
    public static class DependencyInjection
    {
        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("travelnize-database");

            return services;
        }
    }
}
