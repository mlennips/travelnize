namespace LIT.Travelnize.Domain.Trips
{
    public static class TripErrors
    {
        // Trip
        public static readonly ErrorDetail TripNotFound = new("Trip.NotFound", "Trip not found.");
        public static readonly ErrorDetail AtLeastOneOrganisatorRequired = new("Trip.AtLeastOneOrganisatorRequired", "At least one organisator required.");

        // Participant
        public static readonly ErrorDetail ParticipantNotFound = new("Participant.NotFound", "Participant not found.");
        public static readonly ErrorDetail CannotChangePermissionOfGuestParticipant = new("Participant.CannotChangePermissionOfGuest", "Cannot change permission of guest participant.");

        // Transportation
        public static readonly ErrorDetail TransportationNotFound = new("Transportation.NotFound", "Transport konnte nicht gefunden werden.");
        public static readonly ErrorDetail InvalidTransportationDates = new("Transportation.InvalidDates", "Departure date must be before arrival date.");

        // TravelSegment
        public static readonly ErrorDetail TravelSegmentNotFound = new("TravelSegment.NotFound", "Travel segment not found.");

        // Destination
        public static readonly ErrorDetail DestinationNotFound = new("Destination.NotFound", "Destination not found.");
        public static readonly ErrorDetail DestinationAlreadyExistsInSegment = new("Destination.AlreadyExistsInSegment", "Destination already exists in segment.");
        public static readonly ErrorDetail DestinationDateRangeOutOfSegmentRange = new("Destination.DateRangeOutOfSegmentRange", "Destination date range out of segment range");

        // Accommodation
        public static readonly ErrorDetail AccommodationNotFound = new("Accommodation.NotFound", "Accommodation not found.");
        public static readonly ErrorDetail InvalidAccommodationDates = new("Accommodation.InvalidDates", "Check-in date must be before check-out date.");

        // Activity
        public static readonly ErrorDetail ActivityNotFound = new("Activity.NotFound", "Activity not found.");
    }
}
