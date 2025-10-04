using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class DbContextInitialiser
    {
        private async Task AddDemoTrip6Async()
        {
            // Start am nächsten Samstag
            var today = DateTime.UtcNow.Date;
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
            if (daysUntilSaturday == 0) daysUntilSaturday = 7;
            var tripStart = today.AddDays(daysUntilSaturday);
            var tripEnd = tripStart.AddDays(7);

            var tripSlot = PlanningSlot.Create(tripStart, tripEnd);
            var user = await userManager.FindByEmailAsync("demo@travelnize.de");
            var trip = Trip.Create(
                user!,
                "Klassenfahrt Südtirol",
                "Klassenfahrt mit 3 Organisatoren und 50 Schülern nach Südtirol und München.",
                tripSlot 
            );

            // 3 Organisatoren
            var betreuer1 = trip.AddParticipant(Guid.NewGuid(), "Frau Müller (Lehrerin)", new Email("mueller@schule.de")).Value!;
            var betreuer2 = trip.AddParticipant(Guid.NewGuid(), "Herr Schmidt (Lehrer)", new Email("schmidt@schule.de")).Value!;
            var betreuer3 = trip.AddParticipant(Guid.NewGuid(), "Frau Becker (Betreuerin)", new Email("becker@schule.de")).Value!;
            trip.ChangeParticipantPermission(betreuer1.Id, PermissionLevel.Organisator);
            trip.ChangeParticipantPermission(betreuer2.Id, PermissionLevel.Organisator);
            trip.ChangeParticipantPermission(betreuer3.Id, PermissionLevel.Organisator);

            // 50 Schüler
            for (int i = 1; i <= 50; i++)
            {
                trip.AddParticipantAsGuest($"Schüler {i}", new Email($"schueler{i}@schule.de"));
            }

            // Segment 1: Südtirol (5 Tage)
            var tirolSegment = trip.AddTravelSegment(
                "Südtirol",
                "Natur, Berge und Kultur in Südtirol",
                PlanningSlot.Create(tripStart, tripStart.AddDays(5))
            ).Value!;

            var tirolDestination = trip.AddDestinationToTravelSegment(
                tirolSegment.Id,
                "Bozen, Südtirol",
                "Erkundung der Alpenregion, Natur und italienische Kultur.",
                Location.WithCoordinates(46.4983, 11.3548),
                PlanningSlot.Create(tripStart, tripStart.AddDays(5))
            ).Value!;

            trip.AddAccommodationToDestination(
                tirolDestination.Id,
                "Jugendherberge Bozen",
                AccommodationType.Hostel,
                Address.Empty,
                PlanningSlot.Create(tripStart.AddHours(14), tripStart.AddDays(5).AddHours(9))
            );

            // Aktivitäten Südtirol
            trip.AddActivity(tirolDestination.Id, "Wanderung in den Dolomiten", "Geführte Wanderung für die ganze Klasse.", Location.Empty, tripStart.AddHours(16), TimeSpan.FromHours(3));
            trip.AddActivity(tirolDestination.Id, "Besuch Ötzi-Museum", "Spannende Führung durch das Archäologiemuseum.", Location.Empty, tripStart.AddDays(1).AddHours(14), TimeSpan.FromHours(2));
            trip.AddActivity(tirolDestination.Id, "Italienischer Kochkurs", "Gemeinsames Kochen von Pizza und Pasta.", Location.Empty, tripStart.AddDays(2).AddHours(16), TimeSpan.FromHours(2));
            trip.AddActivity(tirolDestination.Id, "Sportturnier", "Fußball- und Volleyballturnier auf dem Jugendherbergsgelände.", Location.Empty, tripStart.AddDays(3).AddHours(15), TimeSpan.FromHours(3));
            trip.AddActivity(tirolDestination.Id, "Abend am Lagerfeuer", "Lagerfeuer mit Musik und Spielen.", Location.Empty, tripStart.AddDays(4).AddHours(19), TimeSpan.FromHours(2));

            // Segment 2: München (2 Tage)
            var munichSegment = trip.AddTravelSegment(
                "München",
                "Kultur und Spaß in München",
                PlanningSlot.Create(tripStart.AddDays(5), tripEnd)
            ).Value!;

            var munichDestination = trip.AddDestinationToTravelSegment(
                munichSegment.Id,
                "München",
                "Erlebnisreiche Tage in der bayerischen Landeshauptstadt.",
                Location.WithCoordinates(48.1351, 11.5820),
                PlanningSlot.Create(tripStart.AddDays(5), tripEnd)
            ).Value!;

            trip.AddAccommodationToDestination(
                munichDestination.Id,
                "Hostel München City",
                AccommodationType.Hostel,
                Address.Empty,
                PlanningSlot.Create(tripStart.AddDays(5).AddHours(12), tripEnd.AddHours(9))
            );

            // Aktivitäten München
            trip.AddActivity(munichDestination.Id, "Deutsches Museum", "Besuch des größten naturwissenschaftlichen Museums der Welt.", Location.Empty, tripStart.AddDays(5).AddHours(14), TimeSpan.FromHours(3));
            trip.AddActivity(munichDestination.Id, "Stadtrallye", "Teamwettbewerb durch die Münchner Innenstadt.", Location.Empty, tripStart.AddDays(6).AddHours(11), TimeSpan.FromHours(2));
            trip.AddActivity(munichDestination.Id, "Englischer Garten", "Picknick und Freizeit im Englischen Garten.", Location.Empty, tripStart.AddDays(6).AddHours(15), TimeSpan.FromHours(2));
            trip.AddActivity(munichDestination.Id, "Abschlussabend", "Gemeinsamer Abschlussabend mit Spielen im Hostel.", Location.Empty, tripStart.AddDays(6).AddHours(19), TimeSpan.FromHours(2));

            appContext.Trips.Add(trip);
            await appContext.SaveChangesAsync();
        }
    }
}