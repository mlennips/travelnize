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
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(0), DateTime.UtcNow.AddDays(36)); // 26 Tage
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(
                user!,
                "Europa-Roadtrip",
                "Roadtrip durch Skandinavien, West- und Südeuropa",
                tripSlot
            );

            // TravelSegment 1: Skandinavien (14 Tage)
            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(14)),
                "Skandinavien – Natur, Fjorde und nordische Städte"
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Dänemark",
                "Erkunde die dänische Hauptstadt mit Nyhavn, Tivoli und moderner Architektur.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(3))
            ).Value!;

            var destination1_2 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Schweden",
                "Entdecke die schwedische Metropole mit Altstadt, Schärengarten und königlichem Schloss.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(4), tripSlot.Start?.AddDays(8))
            ).Value!;

            var destination1_3 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Norwegen",
                "Erlebe beeindruckende Fjorde, Berge und die Natur Norwegens.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(8), tripSlot.Start?.AddDays(14))
            ).Value!;

            // TravelSegment 2: WestEuropa (5 Tage)
            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(19)),
                "Westeuropa"
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Hamburg",
                "Entdecke den Hafen, Speicherstadt und das Nachtleben.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(15))
            ).Value!;

            var destination2_2 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Amsterdam",
                "Fahre mit dem Fahrrad durch die Grachtenstadt und genieße das bunte Treiben.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(15), tripSlot.Start?.AddDays(17))
            ).Value!;

            var destination2_3 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Paris",
                "Besuche Eiffelturm, Louvre und genieße französisches Flair.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(17), tripSlot.Start?.AddDays(19))
            ).Value!;

            // TravelSegment 3: Südeuropa (7 Tage)
            var travelSegment3 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(19), tripSlot.Start?.AddDays(26)),
                "Südeuropa – Sonne, Meer und mediterranes Flair"
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Barcelona",
                "Erlebe Gaudí, Tapas und das mediterrane Lebensgefühl.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(19), tripSlot.Start?.AddDays(22))
            ).Value!;

            var destination3_2 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Rom",
                "Tauche ein in die Geschichte der ewigen Stadt mit Kolosseum, Vatikan und italienischer Küche.",
                Location.Empty,
                PlanningSlot.Create(tripSlot.Start?.AddDays(22), tripSlot.Start?.AddDays(26))
            ).Value!;

            // 4 Participants
            trip.AddParticipantAsGuest("Anna Müller", new Email("anna.mueller@travelnize.de"));
            trip.AddParticipantAsGuest("Max Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Lisa Schmidt", new Email("lisa.schmidt@travelnize.de"));
            trip.AddParticipantAsGuest("Tom Becker", new Email("tom.becker@travelnize.de"));

            // Accommodations für alle Zeiträume
            // Skandinavien
            trip.AddAccommodationToDestination(destination1_1.Id, "Copenhagen City Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2));
            trip.AddAccommodationToDestination(destination1_1.Id, "Nyhavn Boutique Hostel", AccommodationType.Hostel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(3));

            trip.AddAccommodationToDestination(destination1_2.Id, "Stockholm Waterfront Hotel", AccommodationType.Hotel,
                Address.Empty, tripSlot.Start?.AddDays(4), tripSlot.Start?.AddDays(6));
            trip.AddAccommodationToDestination(destination1_2.Id, "Gamla Stan Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, tripSlot.Start?.AddDays(6), tripSlot.Start?.AddDays(8));

            trip.AddAccommodationToDestination(destination1_3.Id, "Oslo Fjord Apartments", AccommodationType.Apartment,
                Address.Empty, tripSlot.Start?.AddDays(8), tripSlot.Start?.AddDays(11));
            trip.AddAccommodationToDestination(destination1_3.Id, "Bergen Mountain Lodge", AccommodationType.Lodge,
                Address.Empty, tripSlot.Start?.AddDays(11), tripSlot.Start?.AddDays(14));

            // Westeuropa
            trip.AddAccommodationToDestination(destination2_1.Id, "Hamburg Hafen Hotel", AccommodationType.Hotel,
                Address.Empty, tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(15));

            trip.AddAccommodationToDestination(destination2_2.Id, "Amsterdam Canal Apartments", AccommodationType.Apartment,
                Address.Empty, tripSlot.Start?.AddDays(15), tripSlot.Start?.AddDays(17));

            trip.AddAccommodationToDestination(destination2_3.Id, "Paris Montmartre Hostel", AccommodationType.Hostel,
                Address.Empty, tripSlot.Start?.AddDays(17), tripSlot.Start?.AddDays(19));

            // Südeuropa
            trip.AddAccommodationToDestination(destination3_1.Id, "Barcelona Beach Hotel", AccommodationType.Hotel,
                Address.Empty, tripSlot.Start?.AddDays(19), tripSlot.Start?.AddDays(22));

            trip.AddAccommodationToDestination(destination3_2.Id, "Rome Colosseum Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, tripSlot.Start?.AddDays(22), tripSlot.Start?.AddDays(24));
            trip.AddAccommodationToDestination(destination3_2.Id, "Trastevere Boutique Hotel", AccommodationType.Hotel,
                Address.Empty, tripSlot.Start?.AddDays(24), tripSlot.Start?.AddDays(26));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip2Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(40), DateTime.UtcNow.AddDays(61)); // 21 Tage
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(
                user!,
                "Amerika-Roadtrip",
                "Abenteuer quer durch die USA: Von der Westküste über das Herzland bis zur Ostküste.",
                tripSlot
            );

            // TravelSegment 1: Westküste (7 Tage)
            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(7)),
                "Westküste – Kalifornische Highlights"
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "San Francisco",
                "Golden Gate Bridge, Alcatraz und Cable Cars erleben.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(3))
            ).Value!;

            var destination1_2 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Los Angeles",
                "Hollywood, Venice Beach und das pulsierende Stadtleben.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(3), travelSegment1.Slot.Start?.AddDays(7))
            ).Value!;

            // TravelSegment 2: Central (7 Tage)
            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(7), tripSlot.Start?.AddDays(14)),
                "Central – Nationalparks & Route 66"
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Grand Canyon",
                "Atemberaubende Schluchten und Wanderungen.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(3))
            ).Value!;

            var destination2_2 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Las Vegas",
                "Glitzernde Casinos, Shows und Nachtleben.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(3), travelSegment2.Slot.Start?.AddDays(7))
            ).Value!;

            // Neue Destination: Texas (3 Tage, im Central-Segment)
            var destination2_3 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Texas",
                "Erlebe texanische Kultur, BBQ und weite Landschaften.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(5), travelSegment2.Slot.Start?.AddDays(7))
            ).Value!;

            // TravelSegment 3: Ostküste (7 Tage)
            var travelSegment3 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(21)),
                "Ostküste – Metropolen & Geschichte"
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "New York City",
                "Times Square, Central Park und Freiheitsstatue.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(4))
            ).Value!;

            var destination3_2 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Washington D.C.",
                "Weiße Haus, Museen und Monumente.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(4), travelSegment3.Slot.Start?.AddDays(7))
            ).Value!;

            // Neue Destination: Boston (2 Tage, im Ostküsten-Segment)
            var destination3_3 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Boston",
                "Historische Stadt mit Harvard, Freedom Trail und maritimen Flair.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(5), travelSegment3.Slot.Start?.AddDays(7))
            ).Value!;

            // Teilnehmer
            trip.AddParticipantAsGuest("John Miller", new Email("john.miller@travelnize.de"));
            trip.AddParticipantAsGuest("Emily Clark", new Email("emily.clark@travelnize.de"));
            trip.AddParticipantAsGuest("Sarah Lee", new Email("sarah.lee@travelnize.de"));
            trip.AddParticipantAsGuest("David Smith", new Email("david.smith@travelnize.de"));

            // Accommodations
            // Westküste
            trip.AddAccommodationToDestination(destination1_1.Id, "San Francisco Downtown Hostel", AccommodationType.Hostel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2));
            trip.AddAccommodationToDestination(destination1_1.Id, "Golden Gate Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(3));

            trip.AddAccommodationToDestination(destination1_2.Id, "LA Beach Apartments", AccommodationType.Apartment,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(3), travelSegment1.Slot.Start?.AddDays(6));
            trip.AddAccommodationToDestination(destination1_2.Id, "Hollywood Inn", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(6), travelSegment1.Slot.Start?.AddDays(7));

            // Central
            trip.AddAccommodationToDestination(destination2_1.Id, "Grand Canyon Lodge", AccommodationType.Lodge,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(3));

            trip.AddAccommodationToDestination(destination2_2.Id, "Vegas Strip Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start?.AddDays(3), travelSegment2.Slot.Start?.AddDays(5));

            // Unterkunft für Texas
            trip.AddAccommodationToDestination(destination2_3.Id, "Texas Ranch Motel", AccommodationType.Motel,
                Address.Empty, travelSegment2.Slot.Start?.AddDays(5), travelSegment2.Slot.Start?.AddDays(7));

            // Ostküste
            trip.AddAccommodationToDestination(destination3_1.Id, "NYC Central Hostel", AccommodationType.Hostel,
                Address.Empty, travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2));
            trip.AddAccommodationToDestination(destination3_1.Id, "Manhattan Suites", AccommodationType.Hotel,
                Address.Empty, travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(4));

            trip.AddAccommodationToDestination(destination3_2.Id, "Capitol Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, travelSegment3.Slot.Start?.AddDays(4), travelSegment3.Slot.Start?.AddDays(5));

            // Unterkunft für Boston
            trip.AddAccommodationToDestination(destination3_3.Id, "Boston Harbor Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment3.Slot.Start?.AddDays(5), travelSegment3.Slot.Start?.AddDays(7));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip3Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(70), DateTime.UtcNow.AddDays(82)); // 12 Tage
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(
                user!,
                "Großbritannien-Rundreise",
                "Entdecke die Vielfalt Großbritanniens: Von London über die Highlands bis nach Wales.",
                tripSlot
            );

            // TravelSegment 1: London & Umgebung (4 Tage)
            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(4)),
                "London & Umgebung"
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "London",
                "Big Ben, Buckingham Palace und britische Kultur.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4))
            ).Value!;

            // TravelSegment 2: Schottland (5 Tage)
            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(4), tripSlot.Start?.AddDays(9)),
                "Schottland – Highlands & Edinburgh"
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Edinburgh",
                "Historische Altstadt, Edinburgh Castle und Festivals.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(2))
            ).Value!;

            var destination2_2 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Highlands",
                "Atemberaubende Natur, Loch Ness und Burgen.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(2), travelSegment2.Slot.Start?.AddDays(5))
            ).Value!;

            // TravelSegment 3: Wales (3 Tage)
            var travelSegment3 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(9), tripSlot.Start?.AddDays(12)),
                "Wales – Küste & Nationalparks"
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Cardiff",
                "Walisische Hauptstadt, Cardiff Castle und lebendige Musikszene.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(3))
            ).Value!;

            // Teilnehmer
            trip.AddParticipantAsGuest("Oliver Brown", new Email("oliver.brown@travelnize.de"));
            trip.AddParticipantAsGuest("Sophie Evans", new Email("sophie.evans@travelnize.de"));
            trip.AddParticipantAsGuest("Mia Wilson", new Email("mia.wilson@travelnize.de"));
            trip.AddParticipantAsGuest("Jack Taylor", new Email("jack.taylor@travelnize.de"));

            // Accommodations
            // London
            trip.AddAccommodationToDestination(destination1_1.Id, "London City Hostel", AccommodationType.Hostel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2));
            trip.AddAccommodationToDestination(destination1_1.Id, "Westminster Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(4));

            // Edinburgh
            trip.AddAccommodationToDestination(destination2_1.Id, "Edinburgh Old Town Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(2));

            // Highlands
            trip.AddAccommodationToDestination(destination2_2.Id, "Highland Lodge", AccommodationType.Lodge,
                Address.Empty, travelSegment2.Slot.Start?.AddDays(2), travelSegment2.Slot.Start?.AddDays(5));

            // Cardiff
            trip.AddAccommodationToDestination(destination3_1.Id, "Cardiff Bay Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2));
            trip.AddAccommodationToDestination(destination3_1.Id, "Welsh Coast Apartments", AccommodationType.Apartment,
                Address.Empty, travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(3));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip4Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(90), DateTime.UtcNow.AddDays(104)); // 14 Tage
            var user = await userManager.FindByEmailAsync("demo@localhost");
            var trip = Trip.Create(
                user!,
                "Kanada-Abenteuer",
                "Von Vancouver bis Montreal: Natur, Städte und Wildnis Kanadas erleben.",
                tripSlot
            );

            // TravelSegment 1: Westkanada (6 Tage)
            var travelSegment1 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(6)),
                "Westkanada – Vancouver & Rockies"
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Vancouver",
                "Multikulturelle Metropole am Pazifik, Stanley Park und Gastown.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2))
            ).Value!;

            var destination1_2 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Banff Nationalpark",
                "Berge, Seen und Wildtiere in den kanadischen Rockies.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(6))
            ).Value!;

            // TravelSegment 2: Zentral-Kanada (4 Tage)
            var travelSegment2 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(6), tripSlot.Start?.AddDays(10)),
                "Zentral-Kanada – Prärien & Winnipeg"
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Winnipeg",
                "Kunst, Kultur und Geschichte im Herzen Kanadas.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(4))
            ).Value!;

            // TravelSegment 3: Ostkanada (4 Tage)
            var travelSegment3 = trip.AddTravelSegment(
                PlanningSlot.Create(tripSlot.Start?.AddDays(10), tripSlot.Start?.AddDays(14)),
                "Ostkanada – Toronto & Montreal"
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Toronto",
                "CN Tower, Multikulti und Shopping.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2))
            ).Value!;

            var destination3_2 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Montreal",
                "Französisches Flair, Altstadt und Festivals.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(4))
            ).Value!;

            // Teilnehmer
            trip.AddParticipantAsGuest("Marie Tremblay", new Email("marie.tremblay@travelnize.de"));
            trip.AddParticipantAsGuest("Lucas Dubois", new Email("lucas.dubois@travelnize.de"));
            trip.AddParticipantAsGuest("Sophie Martin", new Email("sophie.martin@travelnize.de"));
            trip.AddParticipantAsGuest("Noah Lefevre", new Email("noah.lefevre@travelnize.de"));

            // Accommodations
            // Vancouver
            trip.AddAccommodationToDestination(destination1_1.Id, "Vancouver Downtown Hostel", AccommodationType.Hostel,
                Address.Empty, travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2));

            // Banff
            trip.AddAccommodationToDestination(destination1_2.Id, "Banff Mountain Lodge", AccommodationType.Lodge,
                Address.Empty, travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(6));

            // Winnipeg
            trip.AddAccommodationToDestination(destination2_1.Id, "Winnipeg City Hotel", AccommodationType.Hotel,
                Address.Empty, travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(4));

            // Toronto
            trip.AddAccommodationToDestination(destination3_1.Id, "Toronto Central Apartments", AccommodationType.Apartment,
                Address.Empty, travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2));

            // Montreal
            trip.AddAccommodationToDestination(destination3_2.Id, "Montreal Old Town Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(4));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }
    }
}
