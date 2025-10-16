namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record TransportationDto(
        Guid Id,
        string Name,
        string Description,
        string Identifier,
        Location Departure,
        Location Arrival,
        PlanningSlot Traveltime,
        ResourceReference? RouteWebsite,
        TransportationType Type,
        EntityReference? TargetReference
    )
    {
        public static TransportationDto From(Transportation t) =>
            new(
                t.Id,
                t.Name,
                t.Description,
                t.Identifier,
                t.Departure,
                t.Arrival,
                t.Traveltime,
                t.RouteWebsite,
                t.Type,
                t.TargetReference
            );
    }
}
