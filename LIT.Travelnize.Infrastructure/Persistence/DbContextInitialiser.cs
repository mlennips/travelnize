using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;
using LIT.Travelnize.Infrastructure.Identity;
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

    public partial class DbContextInitialiser(
        AppDbContext appContext,
        IdentityDbContext appIdentityContext,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        private readonly string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        public async Task InitialiseAsync()
        {
            try
            {
                // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
                await appContext.Database.EnsureDeletedAsync(); // early development
                await appContext.Database.EnsureCreatedAsync();
                await appIdentityContext.Database.EnsureDeletedAsync(); // early development
                await appIdentityContext.Database.EnsureCreatedAsync();

#pragma warning disable S125
                //await appContext.Database.MigrateAsync();
                //await appIdentityContext.Database.MigrateAsync();
#pragma warning restore S125
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
            var administratorRole = new IdentityRole<Guid>("Administrator");
            var userRole = new IdentityRole<Guid>("User");
            var guestRole = new IdentityRole<Guid>("Guest");

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }
            if (roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await roleManager.CreateAsync(userRole);
            }
            if (roleManager.Roles.All(r => r.Name != guestRole.Name))
            {
                await roleManager.CreateAsync(guestRole);
            }

            // Default users
            var demoUser = new User
            {
                UserName = "demo",
                Email = "demo@travelnize.de",
                Id = Guid.NewGuid(),
                FirstName = "Dee",
                LastName = "Moo"
            };

            if (userManager.Users.All(u => u.UserName != demoUser.UserName))
            {
                await userManager.CreateAsync(demoUser, "Demo%2025");
                if (!string.IsNullOrWhiteSpace(userRole.Name))
                {
                    await userManager.AddToRolesAsync(demoUser, [userRole.Name]);
                }
            }
        }

        public async Task TrySeedAsync()
        {
            if (env == "Development" || env == "Production")
            {
                var hasData = !appContext.Trips.Any();
                if (hasData)
                {
                    await AddDemoTrip1Async();
                    await AddDemoTrip2Async();
                    await AddDemoTrip3Async();
                    await AddDemoTrip4Async();
                    await AddDemoTrip5Async();
                    await AddDemoTrip6Async();
                    await appContext.SaveChangesAsync();
                }
            }
        }
    }
}
