using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

#pragma warning disable S125 // Sections of code should not be commented out
                //await appContext.Database.MigrateAsync();
                //await appIdentityContext.Database.MigrateAsync();
#pragma warning restore S125 // Sections of code should not be commented out
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to initialze the database.", ex);
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
                throw new InvalidOperationException("Failed to seed the database.", ex);
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
