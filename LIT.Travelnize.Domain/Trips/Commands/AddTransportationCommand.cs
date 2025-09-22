namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddTransportationCommand(
        Guid TripId,
        string Name,
        string Description,
        string Identifier,
        Location Departure,
        Location Arrival,
        DateTime DepartureDate,
        DateTime ArrivalDate,
        ExternalUrl RouteLink,
        TransportationType Type
    ) : ICommand<Guid>;
}