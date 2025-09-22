namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record RemoveTravelSegmentCommand(
        Guid TripId,
        Guid SegmentId
    ) : ICommand;
}