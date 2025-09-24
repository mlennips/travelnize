namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateDestinationCommand(
        Guid TripId,
        Guid SegmentId,
        Guid DestinationId,
        string Name,
        string Description,
        Location Location,
        ResourceReference? Image,
        ResourceReference? Website
    ) : ICommand;
}