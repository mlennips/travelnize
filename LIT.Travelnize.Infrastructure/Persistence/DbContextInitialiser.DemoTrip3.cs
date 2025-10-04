using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class DbContextInitialiser
    {
        private async Task AddDemoTrip3Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(70), DateTime.UtcNow.AddDays(82));
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
            var trip = Trip.Create(
                user!,
                "Großbritannien-Rundreise",
                "Entdecke die Vielfalt Großbritanniens: Von London über die Highlands bis nach Wales.",
                tripSlot
            );

            var travelSegment1 = trip.AddTravelSegment(
                "London & Umgebung",
                "",
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(4))
            ).Value!;

            var destination1_1 = trip.AddDestinationToTravelSegment(
                travelSegment1.Id,
                "London",
                "Big Ben, Buckingham Palace und britische Kultur.",
                Location.Empty,
                PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(4))
            ).Value!;

            var travelSegment2 = trip.AddTravelSegment(
                "Schottland",
                "Highlands & Edinburgh",
                PlanningSlot.Create(tripSlot.Start?.AddDays(4), tripSlot.Start?.AddDays(9))
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

            var travelSegment3 = trip.AddTravelSegment(
                "Wales",
                "Küste & Nationalparks",
                PlanningSlot.Create(tripSlot.Start?.AddDays(9), tripSlot.Start?.AddDays(12))
            ).Value!;

            var destination3_1 = trip.AddDestinationToTravelSegment(
                travelSegment3.Id,
                "Cardiff",
                "Walisische Hauptstadt, Cardiff Castle und lebendige Musikszene.",
                Location.Empty,
                PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(3))
            ).Value!;

            trip.AddParticipantAsGuest("Oliver Brown", new Email("oliver.brown@travelnize.de"));
            trip.AddParticipantAsGuest("Sophie Evans", new Email("sophie.evans@travelnize.de"));
            trip.AddParticipantAsGuest("Mia Wilson", new Email("mia.wilson@travelnize.de"));
            trip.AddParticipantAsGuest("Jack Taylor", new Email("jack.taylor@travelnize.de"));

            trip.AddAccommodationToDestination(destination1_1.Id, "London City Hostel", AccommodationType.Hostel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2)));
            trip.AddAccommodationToDestination(destination1_1.Id, "Westminster Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(4)));

            trip.AddAccommodationToDestination(destination2_1.Id, "Edinburgh Old Town Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(2)));

            trip.AddAccommodationToDestination(destination2_2.Id, "Highland Lodge", AccommodationType.Lodge,
                Address.Empty, PlanningSlot.Create(travelSegment2.Slot.Start?.AddDays(2), travelSegment2.Slot.Start?.AddDays(5)));

            trip.AddAccommodationToDestination(destination3_1.Id, "Cardiff Bay Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2)));
            trip.AddAccommodationToDestination(destination3_1.Id, "Welsh Coast Apartments", AccommodationType.Apartment,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(3)));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }


    }
}