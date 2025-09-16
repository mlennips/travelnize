namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record DestinationDto(
        Guid Id,
        string Name,
        string Description,
        PlanningSlotDto Slot,
        LocationDto Location,
        string? ImageUrl,
        string? Website,
        IReadOnlyList<AccommodationDto> Accommodations
    )
    {
        public static DestinationDto From(Destination d) =>
            new(
                d.Id,
                d.Name,
                d.Description,
                PlanningSlotDto.From(d.Slot),
                LocationDto.From(d.Location),
                d.ImageUrl?.Value,
                d.Website?.Value,
                d.Accommodations.Select(AccommodationDto.From).ToList()
            );
    }
}
