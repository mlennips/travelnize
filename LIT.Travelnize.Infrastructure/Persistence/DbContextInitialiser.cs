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
            //// Default data
            //// Seed, if necessary

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
                }
            }
        }

        private async Task AddDemoTrip1Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(0), DateTime.UtcNow.AddDays(36)); // 26 Tage
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
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
                Location.WithCoordinates(55.78333333, 9.78333333),
                PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4))
            ).Value!;

            var destination1_2 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Schweden",
                "Entdecke die schwedische Metropole mit Altstadt, Schärengarten und königlichem Schloss.",
                Location.WithCoordinates(61.31666667, 14.83333333),
                PlanningSlot.Create(tripSlot.Start?.AddDays(4), tripSlot.Start?.AddDays(8))
            ).Value!;

            var destination1_3 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "Norwegen",
                "Erlebe beeindruckende Fjorde, Berge und die Natur Norwegens.",
                Location.WithCoordinates(62.76666667, 9.45),
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
                Location.WithCoordinates(53.550556, 9.993333),
                PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(15))
            ).Value!;

            var destination2_2 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Amsterdam",
                "Fahre mit dem Fahrrad durch die Grachtenstadt und genieße das bunte Treiben.",
                Location.WithCoordinates(52.37019722, 4.89044444),
                PlanningSlot.Create(tripSlot.Start?.AddDays(15), tripSlot.Start?.AddDays(17))
            ).Value!;

            var destination2_3 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Paris",
                "Besuche Eiffelturm, Louvre und genieße französisches Flair.",
                Location.WithCoordinates(48.85666667, 2.35166667),
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
                Location.WithCoordinates(41.4, 2.16666667),
                PlanningSlot.Create(tripSlot.Start?.AddDays(19), tripSlot.Start?.AddDays(22))
            ).Value!;

            var destination3_2 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Rom",
                "Tauche ein in die Geschichte der ewigen Stadt mit Kolosseum, Vatikan und italienischer Küche.",
                Location.WithCoordinates(41.88333333, 12.48333333),
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

            // Activities
            // Dänemark
            trip.AddActivity(destination1_1.Id, "Stadtrundgang Kopenhagen", "Geführte Tour durch die Altstadt und den Nyhavn.", Location.Empty, travelSegment1.Slot.Start, TimeSpan.FromHours(3));
            trip.AddActivity(destination1_1.Id, "Besuch im Tivoli", "Erlebe den berühmten Freizeitpark Tivoli mit Fahrgeschäften und Shows.", Location.Empty, travelSegment1.Slot.Start?.AddDays(1), TimeSpan.FromHours(5));
            trip.AddActivity(destination1_1.Id, "Fahrradtour", "Entdecke Kopenhagen auf dem Rad – vorbei an moderner Architektur und Parks.", Location.Empty, travelSegment1.Slot.Start?.AddDays(2), TimeSpan.FromHours(2));
            
            // Schweden
            trip.AddActivity(destination1_2.Id, "Altstadt Gamla Stan", "Spaziergang durch die historische Altstadt von Stockholm.", Location.Empty, tripSlot.Start?.AddDays(4), TimeSpan.FromHours(2));
            trip.AddActivity(destination1_2.Id, "Bootstour Schärengarten", "Bootsfahrt durch die Inselwelt vor Stockholm.", Location.Empty, tripSlot.Start?.AddDays(5), TimeSpan.FromHours(4));
            trip.AddActivity(destination1_2.Id, "Besuch Königliches Schloss", "Führung durch das königliche Schloss in Stockholm.", Location.Empty, tripSlot.Start?.AddDays(6), TimeSpan.FromHours(2));
            
            // Norwegen
            trip.AddActivity(destination1_3.Id, "Fjord-Kreuzfahrt", "Bootstour durch die berühmten norwegischen Fjorde.", Location.Empty, tripSlot.Start?.AddDays(8), TimeSpan.FromHours(6));
            trip.AddActivity(destination1_3.Id, "Wanderung Preikestolen", "Atemberaubende Wanderung zum Felsenplateau Preikestolen.", Location.Empty, tripSlot.Start?.AddDays(9), TimeSpan.FromHours(5));
            trip.AddActivity(destination1_3.Id, "Besuch Bergen", "Stadtrundgang durch die Hansestadt Bergen.", Location.Empty, tripSlot.Start?.AddDays(10), TimeSpan.FromHours(3));
            
            // Hamburg
            trip.AddActivity(destination2_1.Id, "Hafenrundfahrt", "Bootstour durch den Hamburger Hafen und die Speicherstadt.", Location.Empty, tripSlot.Start?.AddDays(14), TimeSpan.FromHours(2));
            trip.AddActivity(destination2_1.Id, "Reeperbahn-Tour", "Erkunde das berühmte Hamburger Nachtleben auf der Reeperbahn.", Location.Empty, tripSlot.Start?.AddDays(14).AddHours(20), TimeSpan.FromHours(3));

            // Amsterdam
            trip.AddActivity(destination2_2.Id, "Grachtenfahrt", "Bootsfahrt durch die Kanäle von Amsterdam.", Location.Empty, tripSlot.Start?.AddDays(15), TimeSpan.FromHours(2));
            trip.AddActivity(destination2_2.Id, "Van Gogh Museum", "Besuch des weltberühmten Van Gogh Museums.", Location.Empty, tripSlot.Start?.AddDays(16), TimeSpan.FromHours(2));
            trip.AddActivity(destination2_2.Id, "Fahrradtour", "Geführte Fahrradtour durch die Innenstadt.", Location.Empty, tripSlot.Start?.AddDays(16).AddHours(14), TimeSpan.FromHours(2));

            // Paris
            trip.AddActivity(destination2_3.Id, "Eiffelturm-Besuch", "Auffahrt auf den Eiffelturm mit Panoramablick.", Location.Empty, tripSlot.Start?.AddDays(17), TimeSpan.FromHours(2));
            trip.AddActivity(destination2_3.Id, "Louvre-Führung", "Geführte Tour durch das berühmte Kunstmuseum Louvre.", Location.Empty, tripSlot.Start?.AddDays(18), TimeSpan.FromHours(3));
            trip.AddActivity(destination2_3.Id, "Spaziergang Montmartre", "Erkunde das Künstlerviertel Montmartre und Sacré-Cœur.", Location.Empty, tripSlot.Start?.AddDays(18).AddHours(16), TimeSpan.FromHours(2));
            
            // Barcelona
            trip.AddActivity(destination3_1.Id, "Sagrada Família", "Besichtigung der berühmten Basilika von Gaudí.", Location.Empty, tripSlot.Start?.AddDays(19), TimeSpan.FromHours(2));
            trip.AddActivity(destination3_1.Id, "Tapas-Tour", "Kulinarische Tour durch Barcelonas Tapas-Bars.", Location.Empty, tripSlot.Start?.AddDays(20), TimeSpan.FromHours(3));
            trip.AddActivity(destination3_1.Id, "Strandtag Barceloneta", "Entspannung und Baden am Stadtstrand.", Location.Empty, tripSlot.Start?.AddDays(21), TimeSpan.FromHours(5));
            
            // Rom
            trip.AddActivity(destination3_2.Id, "Kolosseum & Forum Romanum", "Geführte Tour durch das antike Rom.", Location.Empty, tripSlot.Start?.AddDays(22), TimeSpan.FromHours(3));
            trip.AddActivity(destination3_2.Id, "Vatikanische Museen", "Besuch der Vatikanstadt und der Sixtinischen Kapelle.", Location.Empty, tripSlot.Start?.AddDays(23), TimeSpan.FromHours(4));
            trip.AddActivity(destination3_2.Id, "Piazza Navona & Pantheon", "Spaziergang durch das barocke Rom.", Location.Empty, tripSlot.Start?.AddDays(24), TimeSpan.FromHours(2));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

        private async Task AddDemoTrip2Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(40), DateTime.UtcNow.AddDays(61)); // 21 Tage
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
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
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
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
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
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

        private async Task AddDemoTrip5Async()
        {
            // Start: 1. Juni nächsten Jahres, 7 Nächte Kreuzfahrt + 10 Nächte Dänemmark + 2 Nächte Hamburg
            var nextYear = DateTime.UtcNow.Year + 1;
            var tripStart = new DateTime(nextYear, 6, 6, 14, 0, 0, DateTimeKind.Utc);
            var tripEnd = tripStart.AddDays(19); 

            var tripSlot = PlanningSlot.Create(tripStart, tripEnd);
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
            var trip = Trip.Create(
                user!,
                "Elternzeit " + tripStart.Year,
                "Kreuzfahrt ab Kiel und Ferienhaus in Dänemark",
                tripSlot
            );

            #region Cruise

            // Ein TravelSegment für die gesamte Kreuzfahrt
            var cruiseSegment = trip.AddTravelSegment(
                PlanningSlot.Create(tripStart, tripEnd),
                "Kreuzfahrt"
            ).Value!;

            // Eine Destination: "AIDA Kreuzfahrt"
            var cruiseDestination = trip.AddDestinationToTravelSegment(
                cruiseSegment.Id,
                "AIDA Kreuzfahrt - Nordeuropa",
                "Kreuzfahrt mit der AIDA Nova ab/bis Kiel.",
                Location.Empty,
                PlanningSlot.Create(tripStart, tripStart.AddDays(7))
            ).Value!;

            // Accommodation: AIDA Nova für die gesamte Reise
            trip.AddAccommodationToDestination(
                cruiseDestination.Id,
                "AIDA Nova",
                AccommodationType.Cruise,
                Address.Empty,
                tripStart,
                tripEnd
            );

            // Activities pro Tag
            trip.AddActivity(cruiseDestination.Id, "Abfahrt Kiel", "Start der Kreuzfahrt in Kiel", Location.Empty, tripStart, TimeSpan.FromHours(4));
            trip.AddActivity(cruiseDestination.Id, "Seetag", "Entspannung und Aktivitäten an Bord", Location.Empty, tripStart.AddDays(1), TimeSpan.FromHours(24));
            trip.AddActivity(cruiseDestination.Id, "Oslo", "Landgang in Oslo, Norwegen", Location.Empty, tripStart.AddDays(2), TimeSpan.FromHours(10));
            trip.AddActivity(cruiseDestination.Id, "Kristiansand", "Landgang in Kristiansand, Norwegen", Location.Empty, tripStart.AddDays(3), TimeSpan.FromHours(8));
            trip.AddActivity(cruiseDestination.Id, "Skagen", "Landgang in Skagen, Dänemark", Location.Empty, tripStart.AddDays(4), TimeSpan.FromHours(8));
            trip.AddActivity(cruiseDestination.Id, "Kopenhagen", "Landgang in Kopenhagen, Dänemark", Location.Empty, tripStart.AddDays(5), TimeSpan.FromHours(10));
            trip.AddActivity(cruiseDestination.Id, "Arhus", "Landgang in Arhus, Dänemark", Location.Empty, tripStart.AddDays(6), TimeSpan.FromHours(8));
            trip.AddActivity(cruiseDestination.Id, "Ankunft Kiel", "Ende der Kreuzfahrt in Kiel", Location.Empty, tripStart.AddDays(7), TimeSpan.FromHours(2));

            #endregion

            #region Denmark

            var denmarkSegment = trip.AddTravelSegment(
                PlanningSlot.Create(tripStart.AddDays(7), tripEnd.AddDays(-2)),
                "Dänemark"
            ).Value!;

            var denmarkDestination = trip.AddDestinationToTravelSegment(
                denmarkSegment.Id,
                "Ost-Dänemark",
                "Erkunde Dänemark",
                Location.Empty,
                PlanningSlot.Create(tripStart.AddDays(7), tripEnd.AddDays(-2))
            ).Value!;

            trip.AddAccommodationToDestination(
                denmarkDestination.Id,
                "Ferienhaus am Meer",
                AccommodationType.Villa,
                Address.Empty,
                tripStart.AddDays(7),
                tripEnd.AddDays(-2)
            );

            #endregion


            #region Hamburg

            var hamburgSegment = trip.AddTravelSegment(
                PlanningSlot.Create(tripEnd.AddDays(-2), tripEnd),
                "Rückreise"
            ).Value!;

            var hamburgDestination = trip.AddDestinationToTravelSegment(
                hamburgSegment.Id,
                "Hamburg",
                "Erkunde Hamburg",
                Location.Empty,
                PlanningSlot.Create(tripEnd.AddDays(-2), tripEnd)
            ).Value!;

            trip.AddAccommodationToDestination(
                hamburgDestination.Id,
                "Hotel in Hamburg",
                AccommodationType.Hotel,
                Address.Empty,
                tripEnd.AddDays(-2),
                tripEnd
            );

            #endregion


            // Teilnehmer
            trip.AddParticipantAsGuest("Max Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Ute Mustermann", new Email("ute.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Anton Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Merle Mustermann", new Email("max.mustermann@travelnize.de"));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

    }
}
