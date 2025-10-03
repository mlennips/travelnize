namespace LIT.Travelnize.Helpers
{
    public class LinkHelper
    {
        // Trips
        public static string TripLink(Guid tripId) => $"/trips/{tripId}";
        public static string TripLinkCreate() => $"/trips/create";
        public static string TripLinkEdit(Guid tripId) => $"/trips/{tripId}/edit";

        // TravelSegment
        public static string TravelSegmentLink(Guid tripId, Guid travelSegmentId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}";
        public static string TravelSegmentLinkCreate(Guid tripId) => $"/trips/{tripId}/travelsegments/create";
        public static string TravelSegmentLinkEdit(Guid tripId, Guid travelSegmentId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/edit";

        // Destinations
        public static string DestinationLink(Guid tripId, Guid travelSegmentId, Guid destinationId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}";
        public static string DestinationLinkCreate(Guid tripId, Guid travelSegmentId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/create";
        public static string DestinationLinkEdit(Guid tripId, Guid travelSegmentId, Guid destinationId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/edit";

        // Accommodations
        public static string AccommodationLink(Guid tripId, Guid travelSegmentId, Guid destinationId, Guid accommodationId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/accommodations/{accommodationId}";
        public static string AccommodationLinkCreate(Guid tripId, Guid travelSegmentId, Guid destinationId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/accommodations/create";
        public static string AccommodationLinkEdit(Guid tripId, Guid travelSegmentId, Guid destinationId, Guid accommodationId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/accommodations/{accommodationId}/edit";
        
        // Activities
        public static string ActivityLink(Guid tripId, Guid travelSegmentId, Guid destinationId, Guid activityId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/activities/{activityId}";
        public static string ActivityLinkCreate(Guid tripId, Guid travelSegmentId, Guid destinationId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/activities/create";
        public static string ActivityLinkEdit(Guid tripId, Guid travelSegmentId, Guid destinationId, Guid activityId) => $"/trips/{tripId}/travelsegments/{travelSegmentId}/destinations/{destinationId}/activities/{activityId}/edit";
    }
}
