using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public static class TripsErrors
    {
        public static readonly ErrorDetail AtLeastOneAdminRequired = new("Trip.AtLeastOneAdminRequired", "At least one admin required.");
        public static readonly ErrorDetail TravelSegmentNotFound = new("TravelSegment.NotFound", "Travel segment not found.");
        public static readonly ErrorDetail DestinationNotFound = new("Destination.NotFound", "Destination not found.");
        public static readonly ErrorDetail ParticipantNotFound = new("Participant.NotFound", "Participant not found.");
        public static readonly ErrorDetail DestinationAlreadyExistsInSegment = new("Destination.AlreadyExistsInSegment", "Destination already exists in segment.");
        public static readonly ErrorDetail DestinationDateRangeOutOfSegmentRange = new("Destination.DateRangeOutOfSegmentRange", "Destination date range out of segment range");
        public static readonly ErrorDetail InvalidAccommodationDates = new("Accommodation.InvalidDates", "Check-in date must be before check-out date.");
        public static readonly ErrorDetail InvalidTransportationDates = new("Transportation.InvalidDates", "Departure date must be before arrival date.");
    }
}
