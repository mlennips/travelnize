namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record RemoveAccommodationCommand(
        Guid TripId,
        Guid DestinationId,
        Guid AccommodationId
    ) : ICommand;
}