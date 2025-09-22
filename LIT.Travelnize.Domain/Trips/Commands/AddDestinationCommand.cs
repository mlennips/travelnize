namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddDestinationCommand(
        Guid TripId,
        Guid SegmentId,
        string Name,
        string Description,
        Location Location,
        DateTime? Start,
        DateTime? End
    ) : ICommand<Guid>;
}