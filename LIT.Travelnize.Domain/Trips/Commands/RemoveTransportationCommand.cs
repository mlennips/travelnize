namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record RemoveTransportationCommand(
        Guid TripId,
        Guid TransportationId
    ) : ICommand;
}