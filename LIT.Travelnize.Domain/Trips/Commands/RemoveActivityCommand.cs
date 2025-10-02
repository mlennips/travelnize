namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record RemoveActivityCommand(
        Guid TripId,
        Guid DestinationId,
        Guid ActivityId
    ) : ICommand;
}