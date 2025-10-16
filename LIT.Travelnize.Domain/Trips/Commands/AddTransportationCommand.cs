namespace LIT.Travelnize.Domain.Trips.Commands
{
    public record AddTransportationCommand(
        Guid TripId,
        string Name,
        string Description,
        string Identifier,
        Location Departure,
        Location Arrival,
        PlanningSlot TravelTime,
        ResourceReference RouteWebsite,
        TransportationType Type,
        EntityReference? TargetRefeference
    ) : ICommand<Guid>;
}