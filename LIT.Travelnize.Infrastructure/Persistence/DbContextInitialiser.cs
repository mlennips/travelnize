using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
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

    public class DbContextInitialiser(
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
            var administratorRole = new IdentityRole<Guid>("Administrator");

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            // Default users

            if (env == "Development")
            {
                var administrator = new User
                {
                    UserName = "administrator@localhost",
                    Email = "administrator@localhost",
                    Id = Guid.NewGuid(),
                    FirstName = "Admin",
                    LastName = "Istrator"
                };

                if (userManager.Users.All(u => u.UserName != administrator.UserName))
                {
                    await userManager.CreateAsync(administrator, "%Admin2025");
                    if (!string.IsNullOrWhiteSpace(administratorRole.Name))
                    {
                        await userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
                    }
                }
            }
        }

        public async Task TrySeedAsync()
        {
            //// Default data
            //// Seed, if necessary

            if (env == "Development")
            {
                var hasData = !appContext.Trips.Any();
                if (hasData)
                {
                    await AddDemoTrip1Async();
                    await AddDemoTrip2Async();
                }
            }
        }

        private async Task AddDemoTrip1Async()
        {
            var tripDateRange = new DateRange(DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
            var user = await userManager.FindByEmailAsync("administrator@localhost");
            var trip = Trip.Create(user!, "My first trip", "This is my first trip.",
                tripDateRange);

            var travelSegment = trip.AddTravelSegment(
                new DateRange(tripDateRange.Start, tripDateRange.End),
                "Main travel segment").Value!;

            var destination = trip.AddDestinationToTravelSegment(travelSegment.Id, "Paris", "City of Light", Location.Empty).Value!;

            trip.AddParticipantAsGuest("Guest 1", new Email("guest1@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 2", new Email("guest2@travelnize.de"));

            trip.AddAccommodationToDestination(destination.Id, "Hotel Paris", AccommodationType.Hotel,
                Address.Empty, tripDateRange.Start, tripDateRange.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip2Async()
        {
            var tripDateRange = new DateRange(DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(21));
            var user = await userManager.FindByEmailAsync("administrator@localhost");
            var trip = Trip.Create(user!, "My 2nd trip", "This is my 2nd trip.",
                tripDateRange);

            var travelSegment1 = trip.AddTravelSegment(
                new DateRange(tripDateRange.Start, tripDateRange.Start.AddDays(7)),
                "Part 1").Value!;

            var travelSegment2 = trip.AddTravelSegment(
                new DateRange(travelSegment1.DateRange.End, travelSegment1.DateRange.End.AddDays(7)),
                "Part 2").Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(travelSegment1.Id, "Paris", "City of Light", Location.Empty).Value!;
            var destination1_2 = trip.AddDestinationToTravelSegment(travelSegment1.Id, "London", "Capital of UK", Location.Empty).Value!;
            var destination2_1 = trip.AddDestinationToTravelSegment(travelSegment2.Id, "New York", "The Big Apple", Location.Empty).Value!;

            trip.AddParticipantAsGuest("Guest 1", new Email("guest1@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 2", new Email("guest2@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 3", new Email("guest3@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 4", new Email("guest4@travelnize.de"));

            trip.AddAccommodationToDestination(destination1_1.Id, "Hotel Paris", AccommodationType.Hotel,
                Address.Empty, travelSegment1.DateRange.Start, travelSegment1.DateRange.Start.AddDays(4));
            trip.AddAccommodationToDestination(destination1_2.Id, "Hotel London", AccommodationType.Hotel,
                Address.Empty, travelSegment1.DateRange.Start.AddDays(4), travelSegment1.DateRange.Start.AddDays(3));
            trip.AddAccommodationToDestination(destination2_1.Id, "Hotel New York", AccommodationType.Hotel,
                Address.Empty, travelSegment2.DateRange.Start, travelSegment2.DateRange.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }
    }
}
