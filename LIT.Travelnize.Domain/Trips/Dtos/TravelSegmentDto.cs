namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record TravelSegmentDto(
        Guid Id,
        string Name,
        string Description,
        PlanningSlot Slot,
        IReadOnlyList<DestinationDto> Destinations
    )
    {
        public static TravelSegmentDto From(TravelSegment segment) =>
            new(segment.Id, segment.Name, segment.Description, 
                segment.Slot, 
                segment.Destinations.Select(DestinationDto.From).ToList());
    }
}
