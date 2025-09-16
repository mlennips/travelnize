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

            if (env == "Development")
            {
                var demoUser = new User
                {
                    UserName = "demo@localhost",
                    Email = "demo@localhost",
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
                    await AddDemoTrip3Async();
                }
            }
        }

        private async Task AddDemoTrip1Async()
        {
            var tripSlot = PlanningSlot.Create(0, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(user!, "My first trip", "This is my first trip.",
                tripSlot);

            var travelSegment = trip.AddTravelSegment(
                PlanningSlot.Create(0, tripSlot.Start, tripSlot.End),
                "Main travel segment").Value!;

            var destination = trip.AddDestinationToTravelSegment(travelSegment.Id, "Paris", "City of Light", Location.Empty).Value!;

            trip.AddParticipantAsGuest("Guest 1", new Email("guest1@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 2", new Email("guest2@travelnize.de"));

            trip.AddAccommodationToDestination(destination.Id, "Hotel Paris", AccommodationType.Hotel,
                Address.Empty, tripSlot.Start, tripSlot.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip2Async()
        {
            var tripSlot = PlanningSlot.Create(0, DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(21));
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(user!, "My 2nd trip", "This is my 2nd trip.",
                tripSlot);

            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(0, tripSlot.Start, tripSlot.Start?.AddDays(7)),
                "Part 1").Value!;

            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(0, travelSegment1.Slot.End!.Value, travelSegment1.Slot.End!.Value.AddDays(7)),
                "Part 2").Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(travelSegment1.Id, "Paris", "City of Light", Location.Empty).Value!;
            var destination1_2 = trip.AddDestinationToTravelSegment(travelSegment1.Id, "London", "Capital of UK", Location.Empty).Value!;
            var destination2_1 = trip.AddDestinationToTravelSegment(travelSegment2.Id, "New York", "The Big Apple", Location.Empty).Value!;

            trip.AddParticipantAsGuest("Guest 1", new Email("guest1@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 2", new Email("guest2@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 3", new Email("guest3@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 4", new Email("guest4@travelnize.de"));

            trip.AddAccommodationToDestination(destination1_1.Id, "Hotel Paris", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4));
            trip.AddAccommodationToDestination(destination1_2.Id, "Hotel London", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(4), travelSegment1.Slot.Start?.AddDays(3));
            trip.AddAccommodationToDestination(destination2_1.Id, "Hotel New York", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip3Async()
        {
            var tripSlot = PlanningSlot.Create(0, DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(34));
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(user!, "My 3nd trip", "This is my 3nd trip.",
                tripSlot);

            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(0, tripSlot.Start, tripSlot.Start?.AddDays(7)),
                "Part 1").Value!;

            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(0, travelSegment1.Slot.End!.Value, travelSegment1.Slot.End!.Value.AddDays(7)),
                "Part 2").Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(travelSegment1.Id, "Paris", "City of Light", Location.Empty).Value!;
            var destination1_2 = trip.AddDestinationToTravelSegment(travelSegment1.Id, "London", "Capital of UK", Location.Empty).Value!;
            var destination2_1 = trip.AddDestinationToTravelSegment(travelSegment2.Id, "New York", "The Big Apple", Location.Empty).Value!;

            trip.AddParticipantAsGuest("Guest 1", new Email("guest1@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 2", new Email("guest2@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 3", new Email("guest3@travelnize.de"));
            trip.AddParticipantAsGuest("Guest 4", new Email("guest4@travelnize.de"));

            trip.AddAccommodationToDestination(destination1_1.Id, "Hotel Paris", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4));
            trip.AddAccommodationToDestination(destination1_2.Id, "Hotel London", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(4), travelSegment1.Slot.Start?.AddDays(3));
            trip.AddAccommodationToDestination(destination2_1.Id, "Hotel New York", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }
    }
}
