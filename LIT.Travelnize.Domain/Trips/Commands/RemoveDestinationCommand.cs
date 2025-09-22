namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record RemoveDestinationCommand(
        Guid TripId,
        Guid SegmentId,
        Guid DestinationId
    ) : ICommand;
}