using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class DbContextInitialiser
    {
        private async Task AddDemoTrip5Async()
        {
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

            var cruiseSegment = trip.AddTravelSegment(
                "Kreuzfahrt",
                "",
                PlanningSlot.Create(tripStart, tripEnd)
            ).Value!;

            var cruiseDestination = trip.AddDestinationToTravelSegment(
                cruiseSegment.Id,
                "AIDA Kreuzfahrt - Nordeuropa",
                "Kreuzfahrt mit der AIDA Nova ab/bis Kiel.",
                Location.Empty,
                PlanningSlot.Create(tripStart, tripStart.AddDays(7))
            ).Value!;

            trip.AddAccommodationToDestination(
                cruiseDestination.Id,
                "AIDA Nova",
                AccommodationType.Cruise,
                Address.Empty,
                PlanningSlot.Create(tripStart, tripEnd)
            );

            var denmarkSegment = trip.AddTravelSegment(
                "Dänemark",
                "",
                PlanningSlot.Create(tripStart.AddDays(7), tripEnd.AddDays(-2))
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
                PlanningSlot.Create(tripStart.AddDays(7), tripEnd.AddDays(-2))
            );

            var hamburgSegment = trip.AddTravelSegment(
                "Rückreise",
                "",
                PlanningSlot.Create(tripEnd.AddDays(-2), tripEnd)
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
                PlanningSlot.Create(tripEnd.AddDays(-2), tripEnd)
            );

            trip.AddParticipantAsGuest("Max Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Ute Mustermann", new Email("ute.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Anton Mustermann", new Email("max.mustermann@travelnize.de"));
            trip.AddParticipantAsGuest("Merle Mustermann", new Email("max.mustermann@travelnize.de"));

            appContext.Trips.Add(trip);

            await appContext.SaveChangesAsync();
        }
    }
}