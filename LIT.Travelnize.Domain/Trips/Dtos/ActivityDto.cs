using System;
using LIT.Travelnize.Domain.Trips;

namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record ActivityDto(
        Guid Id,
        Guid TripId,
        Guid TravelSegmentId,
        Guid DestinationId,
        string Name,
        string Description,
        DateTime? Date,
        TimeSpan? Duration,
        Location Location
    )
    {
        public static ActivityDto From(Activity activity) =>
            new(
                activity.Id,
                activity.TripId,
                activity.TravelSegmentId,
                activity.DestinationId,
                activity.Name,
                activity.Description,
                activity.Date,
                activity.Duration,
                activity.Location
            );
    }
}