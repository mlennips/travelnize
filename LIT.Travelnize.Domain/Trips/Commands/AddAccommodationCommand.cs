namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddAccommodationCommand(
        Guid TripId,
        Guid DestinationId,
        string Name,
        AccommodationType Type,
        Address Address,
        DateTime? CheckIn,
        DateTime? CheckOut
    ) : ICommand<Guid>;
}