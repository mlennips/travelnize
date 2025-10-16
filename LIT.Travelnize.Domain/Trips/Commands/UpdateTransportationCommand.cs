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
        PlanningSlot TravelTime,
        ResourceReference RouteWebsite,
        TransportationType Type,
        Guid[] PassengerIds,
        EntityReference? TargetReference
    ) : ICommand;
}