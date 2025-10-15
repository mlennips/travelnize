using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class DbContextInitialiser
    {
        private async Task AddDemoTrip1Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(24));
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
            var trip = Trip.Create(
                user!,
                "Europa-Roadtrip",
                "Roadtrip durch Skandinavien, West- und Südeuropa"
            );

            var travelSegment1 = trip.AddTravelSegment(
                "Skandinavien",
                "Natur, Fjorde und nordische Städte",
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(14))
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

            var travelSegment2 = trip.AddTravelSegment(
                "Westeuropa",
                "",
                PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(19))
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

            var travelSegment3 = trip.AddTravelSegment(
                "Südeuropa",
                "Sonne, Meer und mediterranes Flair",
                PlanningSlot.Create(tripSlot.Start?.AddDays(19), tripSlot.Start?.AddDays(26))
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

            trip.AddParticipantAsGuest("Anna Müller", new Email("anna.mueller@travelnize.de"));
            trip.AddParticipantAsGuest("Max Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Lisa Schmidt", new Email("lisa.schmidt@travelnize.de"));
            trip.AddParticipantAsGuest("Tom Becker", new Email("tom.becker@travelnize.de"));

            // Accommodations (angepasst: PlanningSlot statt Start/Ende einzeln)
            trip.AddAccommodationToDestination(destination1_1.Id, "Copenhagen City Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2)));
            trip.AddAccommodationToDestination(destination1_1.Id, "Nyhavn Boutique Hostel", AccommodationType.Hostel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(3)));

            trip.AddAccommodationToDestination(destination1_2.Id, "Stockholm Waterfront Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(4), tripSlot.Start?.AddDays(6)));
            trip.AddAccommodationToDestination(destination1_2.Id, "Gamla Stan Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(6), tripSlot.Start?.AddDays(8)));

            trip.AddAccommodationToDestination(destination1_3.Id, "Oslo Fjord Apartments", AccommodationType.Apartment,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(8), tripSlot.Start?.AddDays(11)));
            trip.AddAccommodationToDestination(destination1_3.Id, "Bergen Mountain Lodge", AccommodationType.Lodge,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(11), tripSlot.Start?.AddDays(14)));

            trip.AddAccommodationToDestination(destination2_1.Id, "Hamburg Hafen Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(15)));

            trip.AddAccommodationToDestination(destination2_2.Id, "Amsterdam Canal Apartments", AccommodationType.Apartment,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(15), tripSlot.Start?.AddDays(17)));

            trip.AddAccommodationToDestination(destination2_3.Id, "Paris Montmartre Hostel", AccommodationType.Hostel,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(17), tripSlot.Start?.AddDays(19)));

            trip.AddAccommodationToDestination(destination3_1.Id, "Barcelona Beach Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(19), tripSlot.Start?.AddDays(22)));

            trip.AddAccommodationToDestination(destination3_2.Id, "Rome Colosseum Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(22), tripSlot.Start?.AddDays(24)));
            trip.AddAccommodationToDestination(destination3_2.Id, "Trastevere Boutique Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(tripSlot.Start?.AddDays(24), tripSlot.Start?.AddDays(26)));

            // Activities (unverändert)
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
        }

    }
}