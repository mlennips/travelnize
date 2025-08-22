using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public static class TripsErrors
    {
        public static readonly ErrorDetail TripSegmentNotFound = new("TripSegment.NotFound", "Trip segment not found.");
        public static readonly ErrorDetail ParticipantNotFound = new("Participant.NotFound", "Participant not found.");
    }
}
