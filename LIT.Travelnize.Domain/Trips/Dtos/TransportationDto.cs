namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record TransportationDto(
        Guid Id,
        string Name,
        string Description,
        string Identifier,
        LocationDto Departure,
        LocationDto Arrival,
        DateTime DepartureDate,
        DateTime ArrivalDate,
        string? RouteLink,
        string Type
    )
    {
        public static TransportationDto From(Transportation t) =>
            new(
                t.Id,
                t.Name,
                t.Description,
                t.Identifier,
                LocationDto.From(t.Departure),
                LocationDto.From(t.Arrival),
                t.DepartureDate,
                t.ArrivalDate,
                t.RouteLink?.Value,
                t.Type.ToString()
            );
    }
}
