using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class DbContextInitialiser
    {
        private async Task AddDemoTrip4Async()
        {
            var tripSlot = PlanningSlot.Create(DateTime.UtcNow.AddDays(90), DateTime.UtcNow.AddDays(104));
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
            var trip = Trip.Create(
                user!,
                "Kanada-Abenteuer",
                "Von Vancouver bis Montreal: Natur, Städte und Wildnis Kanadas erleben.",
                tripSlot
            );

            var travelSegment1 = trip.AddTravelSegment(
                "Westkanada",
                "Vancouver & Rockies",
                PlanningSlot.Create(tripSlot.Start, tripSlot.Start?.AddDays(6))
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

            var travelSegment2 = trip.AddTravelSegment(
                "Zentral-Kanada",
                "Prärien & Winnipeg",
                PlanningSlot.Create(tripSlot.Start?.AddDays(6), tripSlot.Start?.AddDays(10))
            ).Value!;

            var destination2_1 = trip.AddDestinationToTravelSegment(
                travelSegment2.Id,
                "Winnipeg",
                "Kunst, Kultur und Geschichte im Herzen Kanadas.",
                Location.Empty,
                PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(4))
            ).Value!;

            var travelSegment3 = trip.AddTravelSegment(
                "Ostkanada",
                "Toronto & Montreal",
                PlanningSlot.Create(tripSlot.Start?.AddDays(10), tripSlot.Start?.AddDays(14))
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

            trip.AddParticipantAsGuest("Marie Tremblay", new Email("marie.tremblay@travelnize.de"));
            trip.AddParticipantAsGuest("Lucas Dubois", new Email("lucas.dubois@travelnize.de"));
            trip.AddParticipantAsGuest("Sophie Martin", new Email("sophie.martin@travelnize.de"));
            trip.AddParticipantAsGuest("Noah Lefevre", new Email("noah.lefevre@travelnize.de"));

            trip.AddAccommodationToDestination(destination1_1.Id, "Vancouver Downtown Hostel", AccommodationType.Hostel,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start, travelSegment1.Slot.Start?.AddDays(2)));

            trip.AddAccommodationToDestination(destination1_2.Id, "Banff Mountain Lodge", AccommodationType.Lodge,
                Address.Empty, PlanningSlot.Create(travelSegment1.Slot.Start?.AddDays(2), travelSegment1.Slot.Start?.AddDays(6)));

            trip.AddAccommodationToDestination(destination2_1.Id, "Winnipeg City Hotel", AccommodationType.Hotel,
                Address.Empty, PlanningSlot.Create(travelSegment2.Slot.Start, travelSegment2.Slot.Start?.AddDays(4)));

            trip.AddAccommodationToDestination(destination3_1.Id, "Toronto Central Apartments", AccommodationType.Apartment,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start, travelSegment3.Slot.Start?.AddDays(2)));

            trip.AddAccommodationToDestination(destination3_2.Id, "Montreal Old Town Guesthouse", AccommodationType.Guesthouse,
                Address.Empty, PlanningSlot.Create(travelSegment3.Slot.Start?.AddDays(2), travelSegment3.Slot.Start?.AddDays(4)));

            appContext.Trips.Add(trip);
        }
    }
}