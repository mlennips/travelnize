namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateTravelSegmentCommand(
        Guid TripId,
        Guid SegmentId,
        DateTime Start,
        DateTime End,
        string Description
    ) : ICommand;
}