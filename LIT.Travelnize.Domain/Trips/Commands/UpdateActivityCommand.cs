namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateActivityCommand(
        Guid TripId,
        Guid SegmentId,
        Guid DestinationId,
        Guid ActivityId,
        string Name,
        string Description,
        Location Location,
        DateTime? Date,
        TimeSpan? Duration
    ) : ICommand;
}