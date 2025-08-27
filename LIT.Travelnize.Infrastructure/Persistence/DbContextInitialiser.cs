using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var initialiser = scope.ServiceProvider.GetRequiredService<DbContextInitialiser>();

            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
        }
    }

    public class DbContextInitialiser(
        ILogger<DbContextInitialiser> logger,
        AppDbContext appContext,
        AppIdentityDbContext appIdentityContext,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        public async Task InitialiseAsync()
        {
            try
            {
                // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
                await appContext.Database.EnsureDeletedAsync(); // early development
                await appContext.Database.EnsureCreatedAsync();
                await appIdentityContext.Database.EnsureDeletedAsync(); // early development
                await appIdentityContext.Database.EnsureCreatedAsync();

                //await appContext.Database.MigrateAsync();
                //await appIdentityContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedIdentityAsync();
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedIdentityAsync()
        {
            // Default roles
            var administratorRole = new IdentityRole("Administrator");

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            // Default users
            var administrator = new IdentityUser
            {
                UserName = "administrator@localhost",
                Email = "administrator@localhost",
                Id = Guid.NewGuid().ToString()                
            };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "%Administrator2025");
                if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                {
                    await userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
                }
            }
        }

        public async Task TrySeedAsync()
        {
            //// Default data
            //// Seed, if necessary
            if (true)
            {
                await appContext.SaveChangesAsync();
            }
        }
    }
}
