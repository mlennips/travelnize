namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateAccommodationCommand(
        Guid TripId,
        Guid DestinationId,
        Guid AccommodationId,
        string Name,
        AccommodationType Type,
        Address Address,
        PlanningSlot CheckInOut
    ) : ICommand;
}