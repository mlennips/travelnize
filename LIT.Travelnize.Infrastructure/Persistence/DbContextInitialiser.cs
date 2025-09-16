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
                    await AddDemoTrip4Async();
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
            var tripSlot = PlanningSlot.Create(0, DateTime.UtcNow.AddDays(20), DateTime.UtcNow.AddDays(46)); // 14+7+5=26 Tage
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(
                user!,
                "Sommerurlaub Europa & USA",
                "Sommerurlaub mit Sightseeing, Kultur und Entspannung in Paris, London, New York und Berlin.",
                tripSlot
            );

            // TravelSegment 1: 14 Tage, 2 Destinationen je 7 Tage
            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(0, tripSlot.Start, tripSlot.Start?.AddDays(14)),
                "Kulturelle Highlights in Paris & London"
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Paris",
                "Entdecke die romantische Stadt an der Seine mit Eiffelturm, Louvre und französischer Küche.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(7))
            ).Value!;

            var destination1_2 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "London",
                "Erlebe die britische Hauptstadt mit Big Ben, Buckingham Palace und traditionellen Pubs.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment1.Slot.Start?.AddDays(7), travelSegment1.Slot.End)
            ).Value!;

            // TravelSegment 2: 7 Tage, 1 Destination
            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(0, travelSegment1.Slot.End!.Value, travelSegment1.Slot.End!.Value.AddDays(7)),
                "Städteabenteuer in New York"
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "New York",
                "Erkunde die pulsierende Metropole mit Central Park, Times Square und beeindruckender Skyline.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment2.Slot.Start, travelSegment2.Slot.End)
            ).Value!;

            // TravelSegment 3: 5 Tage, 1 Destination
            var travelSegment3 = trip.AddTravelSegment(
                PlanningSlot.Create(0, travelSegment2.Slot.End!.Value, travelSegment2.Slot.End!.Value.AddDays(5)),
                "Entspannung und Kultur in Berlin"
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Berlin",
                "Genieße die entspannte Atmosphäre, besuche Museen und entdecke das Berliner Nachtleben.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment3.Slot.Start, travelSegment3.Slot.End)
            ).Value!;

            // 4 Participants
            trip.AddParticipantAsGuest("Anna Müller", new Email("anna.mueller@travelnize.de"));
            trip.AddParticipantAsGuest("Max Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Lisa Schmidt", new Email("lisa.schmidt@travelnize.de"));
            trip.AddParticipantAsGuest("Tom Becker", new Email("tom.becker@travelnize.de"));

            // Accommodations für alle Zeiträume
            trip.AddAccommodationToDestination(destination1_1.Id, "Hotel de Paris", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(7));
            trip.AddAccommodationToDestination(destination1_2.Id, "The Londoner", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(7), travelSegment1.Slot.End);

            trip.AddAccommodationToDestination(destination2_1.Id, "NYC Central Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.End);

            trip.AddAccommodationToDestination(destination3_1.Id, "Berlin City Apartments", AccommodationType.Hotel,
                Address.Empty, travelSegment3.Slot.Start, travelSegment3.Slot.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip4Async()
        {
            var tripSlot = PlanningSlot.Create(0, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(34)); // 24 Tage
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(
                user!,
                "Europa-Roadtrip",
                "Ein Roadtrip durch Skandinavien, Westeuropa und Südeuropa mit atemberaubenden Landschaften, Kultur und kulinarischen Highlights.",
                tripSlot
            );

            // TravelSegment 1: Skandinavien (8 Tage)
            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(0, tripSlot.Start, tripSlot.Start?.AddDays(8)),
                "Skandinavien – Natur, Fjorde und nordische Städte"
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Kopenhagen",
                "Erkunde die dänische Hauptstadt mit Nyhavn, Tivoli und moderner Architektur.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4))
            ).Value!;

            var destination1_2 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Stockholm",
                "Entdecke die schwedische Metropole mit Altstadt, Schärengarten und königlichem Schloss.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment1.Slot.Start?.AddDays(4), travelSegment1.Slot.End)
            ).Value!;

            // TravelSegment 2: WestEuropa (8 Tage)
            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(0, travelSegment1.Slot.End!.Value, travelSegment1.Slot.End!.Value.AddDays(8)),
                "Westeuropa – Kultur, Geschichte und Genuss"
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Amsterdam",
                "Fahre mit dem Fahrrad durch die Grachtenstadt und genieße das bunte Treiben.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(4))
            ).Value!;

            var destination2_2 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Paris",
                "Erlebe die Stadt der Liebe mit Eiffelturm, Louvre und französischer Küche.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment2.Slot.Start?.AddDays(4), travelSegment2.Slot.End)
            ).Value!;

            // TravelSegment 3: Südeuropa (8 Tage)
            var travelSegment3 = trip.AddTravelSegment(
                PlanningSlot.Create(0, travelSegment2.Slot.End!.Value, travelSegment2.Slot.End!.Value.AddDays(8)),
                "Südeuropa – Sonne, Meer und mediterranes Flair"
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Barcelona",
                "Genieße Tapas, die Sagrada Familia und das mediterrane Lebensgefühl.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(4))
            ).Value!;

            var destination3_2 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Rom",
                "Tauche ein in die Geschichte der ewigen Stadt mit Kolosseum, Vatikan und italienischer Küche.",
                Location.Empty,
                PlanningSlot.Create(0, travelSegment3.Slot.Start?.AddDays(4), travelSegment3.Slot.End)
            ).Value!;

            // 4 Participants
            trip.AddParticipantAsGuest("Anna Müller", new Email("anna.mueller@travelnize.de"));
            trip.AddParticipantAsGuest("Max Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Lisa Schmidt", new Email("lisa.schmidt@travelnize.de"));
            trip.AddParticipantAsGuest("Tom Becker", new Email("tom.becker@travelnize.de"));

            // Accommodations für alle Zeiträume
            trip.AddAccommodationToDestination(destination1_1.Id, "Copenhagen City Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4));
            trip.AddAccommodationToDestination(destination1_2.Id, "Stockholm Waterfront Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(4), travelSegment1.Slot.End);

            trip.AddAccommodationToDestination(destination2_1.Id, "Amsterdam Canal Apartments", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(4));
            trip.AddAccommodationToDestination(destination2_2.Id, "Hotel de Paris", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start?.AddDays(4), travelSegment2.Slot.End);

            trip.AddAccommodationToDestination(destination3_1.Id, "Barcelona Beach Resort", AccommodationType.Hotel,
                Address.Empty, travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(4));
            trip.AddAccommodationToDestination(destination3_2.Id, "Roma Centro Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment3.Slot.Start?.AddDays(4), travelSegment3.Slot.End);

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }
    }
}
