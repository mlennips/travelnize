namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record UpdateTransportationCommand(
        Guid TripId,
        Guid TransportationId,
        string Name,
        string Description,
        string Identifier,
        Location Departure,
        Location Arrival,
        DateTime DepartureDate,
        DateTime ArrivalDate,
        ResourceReference RouteWebsite,
        TransportationType Type,
        Guid[] PassengerIds
    ) : ICommand;
}