using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class DbContextInitialiser
    {
        private async Task AddDemoTrip2Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(40), DateTime.UtcNow.AddDays(61));
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
            var trip = Trip.Create(
                user!,
                "Amerika-Roadtrip",
                "Abenteuer quer durch die USA: Von der Westküste über das Herzland bis zur Ostküste.",
                tripSlot
            );

            var travelSegment1 = trip.AddTravelSegment(
                "Westküste",
                "Kalifornische Highlights",
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(7))
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

            var travelSegment2 = trip.AddTravelSegment(
                "Central Nationalparks & Route 66",
                "",
                PlanningSlot.Create(tripSlot.Start?.AddDays(7), tripSlot.Start?.AddDays(14))
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

            var destination2_3 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Texas",
                "Erlebe texanische Kultur, BBQ und weite Landschaften.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(5), travelSegment2.Slot.Start?.AddDays(7))
            ).Value!;

            var travelSegment3 = trip.AddTravelSegment(
                "Ostküste",
                "Metropolen & Geschichte",
                PlanningSlot.Create(tripSlot.Start?.AddDays(14), tripSlot.Start?.AddDays(21))
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

            var destination3_3 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Boston",
                "Historische Stadt mit Harvard, Freedom Trail und maritimen Flair.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(5), travelSegment3.Slot.Start?.AddDays(7))
            ).Value!;

            trip.AddParticipantAsGuest("John Miller", new Email("john.miller@travelnize.de"));
            trip.AddParticipantAsGuest("Emily Clark", new Email("emily.clark@travelnize.de"));
            trip.AddParticipantAsGuest("Sarah Lee", new Email("sarah.lee@travelnize.de"));
            trip.AddParticipantAsGuest("David Smith", new Email("david.smith@travelnize.de"));

            trip.AddAccommodationToDestination(destination1_1.Id, "San Francisco Downtown Hostel", AccommodationType.Hostel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2)));
            trip.AddAccommodationToDestination(destination1_1.Id, "Golden Gate Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(3)));

            trip.AddAccommodationToDestination(destination1_2.Id, "LA Beach Apartments", AccommodationType.Apartment,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(3), travelSegment1.Slot.Start?.AddDays(6)));
            trip.AddAccommodationToDestination(destination1_2.Id, "Hollywood Inn", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(6), travelSegment1.Slot.Start?.AddDays(7)));

            trip.AddAccommodationToDestination(destination2_1.Id, "Grand Canyon Lodge", AccommodationType.Lodge,
                Address.Empty, PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(3)));

            trip.AddAccommodationToDestination(destination2_2.Id, "Vegas Strip Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(3), travelSegment2.Slot.Start?.AddDays(5)));

            trip.AddAccommodationToDestination(destination2_3.Id, "Texas Ranch Motel", AccommodationType.Motel,
                Address.Empty, PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(5), travelSegment2.Slot.Start?.AddDays(7)));

            trip.AddAccommodationToDestination(destination3_1.Id, "NYC Central Hostel", AccommodationType.Hostel,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2)));
            trip.AddAccommodationToDestination(destination3_1.Id, "Manhattan Suites", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(4)));

            trip.AddAccommodationToDestination(destination3_2.Id, "Capitol Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(4), travelSegment3.Slot.Start?.AddDays(5)));

            trip.AddAccommodationToDestination(destination3_3.Id, "Boston Harbor Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(5), travelSegment3.Slot.Start?.AddDays(7)));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }

    }
}