namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record TransportationDto(
        Guid Id,
        string Name,
        string Description,
        string Identifier,
        Location Departure,
        Location Arrival,
        DateTime DepartureDate,
        DateTime ArrivalDate,
        ResourceReference? RouteWebsite,
        string Type
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
                t.DepartureDate,
                t.ArrivalDate,
                t.RouteWebsite,
                t.Type.ToString()
            );
    }
}
